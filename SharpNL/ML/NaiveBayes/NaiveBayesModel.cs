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

using System;
using System.Linq;
using SharpNL.ML.Model;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Represents a multinomial Naive Bayes classifier model.
    /// </summary>
    public class NaiveBayesModel : AbstractModel {
        protected double[] outcomeTotals;
        protected long vocabulary;

        #region + Constructors .

        public NaiveBayesModel(Context[] parameters, string[] predLabels, string[] outcomeNames)
            : base(parameters, predLabels, outcomeNames) {
            outcomeTotals = InitOutcomeTotals(outcomeNames, parameters);
            evalParameters = new NaiveBayesEvalParameters(parameters, outcomeNames.Length, outcomeTotals,
                predLabels.Length);
            ModelType = ModelType.NaiveBayes;
        }


        public NaiveBayesModel(Context[] parameters, string[] predLabels, IndexHashTable<string> map,
            string[] outcomeNames)
            : base(parameters, map, outcomeNames) {
            outcomeTotals = InitOutcomeTotals(outcomeNames, parameters);
            evalParameters = new NaiveBayesEvalParameters(parameters, outcomeNames.Length, outcomeTotals,
                predLabels.Length);
            ModelType = ModelType.NaiveBayes;
        }

#if DEBUG
        [Obsolete("Use the constructor with the IndexHashTable instead!")]
        public NaiveBayesModel(Context[] parameters, string[] predLabels, string[] outcomeNames, int correctionConstant,
            double correctionParam)
            : base(parameters, predLabels, outcomeNames, correctionConstant, correctionParam) {

            outcomeTotals = InitOutcomeTotals(outcomeNames, parameters);
            evalParameters = new NaiveBayesEvalParameters(parameters, outcomeNames.Length, outcomeTotals,
                predLabels.Length);
            ModelType = ModelType.NaiveBayes;
        }
#endif

        #endregion

        #region . InitOutcomeTotals .

        protected double[] InitOutcomeTotals(string[] names, Context[] parameters) {
            var totals = new double[names.Length];
            foreach (var context in parameters) {
                for (var j = 0; j < context.Outcomes.Length; ++j) {
                    var outcome = context.Outcomes[j];
                    var count = context.Parameters[j];
                    totals[outcome] += count;
                }
            }
            return totals;
        }

        #endregion

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together..</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context) {
            return Eval(context, new double[evalParameters.NumOutcomes]);
        }

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="probs">An array which is populated with the probabilities for each of the different outcomes, all of which sum to 1.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, double[] probs) {
            return Eval(context, null, probs);
        }

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="probs">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, float[] probs) {
            return Eval(context, probs, new double[evalParameters.NumOutcomes]);
        }

        public double[] Eval(string[] context, float[] probs, double[] outsums) {
            var scontexts = new int[context.Length];
            Array.Clear(outsums, 0, outsums.Length);

            for (var i = 0; i < context.Length; i++) {
                scontexts[i] = map[context[i]];
            }
            return Eval(scontexts, probs, outsums, evalParameters);
        }


        public static double[] Eval(int[] context, float[] values, double[] prior, EvalParameters parameters) {
            var probabilities = new LogProbabilities<int>();

            var bayesEvalParameters = parameters as NaiveBayesEvalParameters;
            var outcomeTotals = bayesEvalParameters != null
                ? bayesEvalParameters.OutcomeTotals
                : new double[prior.Length];
            var vocabulary = bayesEvalParameters != null ? bayesEvalParameters.Vocabulary : 0;

            double value = 1;
            for (var ci = 0; ci < context.Length; ci++) {
                if (context[ci] < 0) 
                    continue;

                var predParams = parameters.Parameters[context[ci]];
                var activeOutcomes = predParams.Outcomes;
                var activeParameters = predParams.Parameters;
                if (values != null) {
                    value = values[ci];
                }
                var ai = 0;
                for (var i = 0; i < outcomeTotals.Length && ai < activeOutcomes.Length; ++i) {
                    var oid = activeOutcomes[ai];
                    var numerator = oid == i ? activeParameters[ai++]*value : 0;
                    var denominator = outcomeTotals[i];

                    probabilities.AddIn(i, GetProbability(numerator, denominator, vocabulary, true), 1);
                }
            }

            var total = outcomeTotals.Sum();
            for (var i = 0; i < outcomeTotals.Length; ++i) {
                var numerator = outcomeTotals[i];
                var denominator = total;
                probabilities.AddIn(i, numerator/denominator, 1);
            }
            for (var i = 0; i < outcomeTotals.Length; ++i) {
                prior[i] = probabilities.Get(i);
            }
            return prior;
        }

        private static double GetProbability(double numerator, double denominator, double vocabulary, bool isSmoothed) {
            if (isSmoothed)
                return GetSmoothedProbability(numerator, denominator, vocabulary);

            if (denominator.Equals(0d))
                return 0;

            return 1.0*(numerator)/(denominator);
        }

        private static double GetSmoothedProbability(double numerator, double denominator, double vocabulary) {
            const double delta = 0.05; // Lidstone smoothing
            return 1.0*(numerator + delta)/(denominator + delta*vocabulary);
        }

    }
}