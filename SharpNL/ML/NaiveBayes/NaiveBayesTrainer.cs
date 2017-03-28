//  
//  Copyright 2015 Gustavo J Knuppe (https://github.com/knuppe)
//  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   

using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Trains models using the perceptron algorithm. Each outcome is represented as
    /// a binary perceptron classifier.  This supports standard (int) weighting as well
    /// average weighting as described in: <br />
    /// Discriminative Training Methods for Hidden Markov Models: Theory and Experiments
    /// with the Perceptron Algorithm. Michael Collins, EMNLP 2002.
    /// </summary>
    public class NaiveBayesTrainer : AbstractEventTrainer {
        
        /// <summary>
        /// The number of unique events which occurred in the event set.
        /// </summary>
        private int numUniqueEvents;

        /// <summary>
        /// The number of events in the event set.
        /// </summary>
        private int numEvents;

        /// <summary>
        /// The number of predicates.
        /// </summary>
        private int numPreds;

        /// <summary>
        /// The number of outcomes.
        /// </summary>
        private int numOutcomes;

        /// <summary>
        /// Records the array of predicates seen in each event.
        /// </summary>
        private int[][] contexts;

        /// <summary>
        /// The value associates with each context. If null then context values are assumes to be 1.
        /// </summary>
        private float[][] values;

        /// <summary>
        /// List of outcomes for each event i, in context[i].
        /// </summary>
        private int[] outcomeList;

        /// <summary>
        /// The number of times an event has been seen for each event i, in context[i].
        /// </summary>
        private int[] numTimesEventsSeen;

        /// <summary>
        /// Stores the String names of the outcomes. The NaiveBayes only tracks outcomes
        /// as ints, and so this array is needed to save the model to disk and
        /// thereby allow users to know what the outcome was in human understandable terms.
        /// </summary>
        private string[] outcomeLabels;

        /// <summary>
        /// Stores the String names of the predicates. The NaiveBayes only tracks
        /// predicates as ints, and so this array is needed to save the model to
        /// disk and thereby allow users to know what the outcome was in human
        /// understandable terms.
        /// </summary>
        private string[] predLabels;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesTrainer"/> class.
        /// </summary>
        public NaiveBayesTrainer() : base(null, false) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesTrainer" /> class with the given evaluation monitor.
        /// </summary>
        /// <param name="monitor">The evaluation monitor.</param>
        public NaiveBayesTrainer(Monitor monitor) : base(monitor, false) {

        }

        #endregion



        /// <summary>
        /// Trains the maximum entropy model using the specified <paramref name="events"/>.
        /// </summary>
        /// <param name="events">The training events.</param>
        /// <returns>The trained <see cref="NaiveBayesModel"/> model.</returns>
        public new NaiveBayesModel Train(IObjectStream<Event> events) { // for convenience
            return (NaiveBayesModel)base.Train(events);
        }

        /// <summary>
        /// Execute the training operation.
        /// </summary>
        /// <param name="indexer">The data indexer.</param>
        /// <returns>The trained <see cref="IMaxentModel"/> model.</returns>
        protected override IMaxentModel DoTrain(IDataIndexer indexer) {
            Display("Incorporating indexed data for training...");

            indexer.Execute();

            contexts = indexer.GetContexts();
            values = indexer.Values;
            numTimesEventsSeen = indexer.GetNumTimesEventsSeen();
            numEvents = indexer.GetNumEvents();
            numUniqueEvents = contexts.Length;

            outcomeLabels = indexer.GetOutcomeLabels();
            outcomeList = indexer.GetOutcomeList();

            predLabels = indexer.GetPredLabels();
            numPreds = predLabels.Length;
            numOutcomes = outcomeLabels.Length;

            Display("done.");

            Display("\tNumber of Event Tokens: " + numUniqueEvents);
            Display("\t    Number of Outcomes: " + numOutcomes);
            Display("\t  Number of Predicates: " + numPreds);

            Display("Computing model parameters...");

            // ReSharper disable once CoVariantArrayConversion - we read the parameters ;)
            Context[] finalParameters = FindParameters();

            Display("...done.\n");


            return new NaiveBayesModel(finalParameters, predLabels, outcomeLabels);
        }

        private MutableContext[] FindParameters() {

            var allOutcomesPattern = new int[numOutcomes];
            for (var oi = 0; oi < numOutcomes; oi++)
                allOutcomesPattern[oi] = oi;

            /** Stores the estimated parameter value of each predicate during iteration. */
            var parameters = new MutableContext[numPreds];
            for (var pi = 0; pi < numPreds; pi++) {
                parameters[pi] = new MutableContext(allOutcomesPattern, new double[numOutcomes]);
                for (var aoi = 0; aoi < numOutcomes; aoi++)
                    parameters[pi].SetParameter(aoi, 0.0);
            }

            // ReSharper disable once CoVariantArrayConversion
            var evalParams = new EvalParameters(parameters, numOutcomes);

            const double stepSize = 1;

            for (var ei = 0; ei < numUniqueEvents; ei++) {
                var targetOutcome = outcomeList[ei];
                for (var ni = 0; ni < numTimesEventsSeen[ei]; ni++) {
                    for (var ci = 0; ci < contexts[ei].Length; ci++) {
                        var pi = contexts[ei][ci];
                        if (values == null) {
                            parameters[pi].UpdateParameter(targetOutcome, stepSize);
                        } else {
                            parameters[pi].UpdateParameter(targetOutcome, stepSize*values[ei][ci]);
                        }
                    }
                }
            }

            // Output the final training stats.
            TrainingStats(evalParams);

            return parameters;

        }

        private void TrainingStats(EvalParameters evalParams) {
            var numCorrect = 0;
            for (var ei = 0; ei < numUniqueEvents; ei++) {
                for (var ni = 0; ni < numTimesEventsSeen[ei]; ni++) {

                    var modelDistribution = new double[numOutcomes];

                    NaiveBayesModel.Eval(contexts[ei], values?[ei], modelDistribution, evalParams);

                    var max = MaxIndex(modelDistribution);
                    if (max == outcomeList[ei])
                        numCorrect++;
                }
            }
            var trainingAccuracy = (double) numCorrect/numEvents;

            Display("Stats: (" + numCorrect + "/" + numEvents + ") " + trainingAccuracy);
        }


        private static int MaxIndex(double[] values) {
            var max = 0;
            for (var i = 1; i < values.Length; i++)
                if (values[i] > values[max])
                    max = i;

            return max;
        }



    }
}