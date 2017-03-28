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
using System.Collections.Generic;
using System.Linq;
using SharpNL.ML.Model;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Abstract parent class for NaiveBayes writers. 
    /// It provides the persist method which takes care of the structure of a stored document, and requires an extending class to define precisely how the data should be stored.
    /// </summary>
    public abstract class NaiveBayesModelWriter : AbstractModelWriter {

        protected Context[] Parameters;
        protected string[] OutcomeLabels;
        protected string[] PredLabels;

        private readonly int numOutcomes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesModelWriter"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        protected NaiveBayesModelWriter(AbstractModel model) {
            var data = model.GetDataStructures();
            numOutcomes = model.GetNumOutcomes();
            Parameters = (Context[]) data[0];
            var map = (IndexHashTable<string>) data[1];
            OutcomeLabels = (string[]) data[2];

            PredLabels = map.ToArray();
        }

        protected ComparablePredicate[] SortValues() {
            var tmpPreds = new ComparablePredicate[Parameters.Length];
            var tmpOutcomes = new int[numOutcomes];
            var tmpParams = new double[numOutcomes];
            var numPreds = 0;

            // Remove parameters with 0 weight and predicates with no parameters
            for (var pid = 0; pid < Parameters.Length; pid++) {
                var numParams = 0;
                var predParams = Parameters[pid].Parameters;
                var outcomePattern = Parameters[pid].Outcomes;
                for (var pi = 0; pi < predParams.Length; pi++) {
                    if (predParams[pi].Equals(0d)) 
                        continue;

                    tmpOutcomes[numParams] = outcomePattern[pi];
                    tmpParams[numParams] = predParams[pi];
                    numParams++;
                }

                var activeOutcomes = new int[numParams];
                var activeParams = new double[numParams];

                for (var pi = 0; pi < numParams; pi++) {
                    activeOutcomes[pi] = tmpOutcomes[pi];
                    activeParams[pi] = tmpParams[pi];
                }
                if (numParams == 0) 
                    continue;

                tmpPreds[numPreds] = new ComparablePredicate(PredLabels[pid], activeOutcomes, activeParams);
                numPreds++;
            }
            //System.err.println("Compressed " + Parameters.Length + " parameters to " + numPreds);

            var sortPreds = new ComparablePredicate[numPreds];
            Array.Copy(tmpPreds, 0, sortPreds, 0, numPreds);
            Array.Sort(sortPreds);

            return sortPreds;
        }


        protected List<List<ComparablePredicate>> ComputeOutcomePatterns(ComparablePredicate[] sorted) {
            var cp = sorted[0];
            var outcomePatterns = new List<List<ComparablePredicate>>();
            var newGroup = new List<ComparablePredicate>();
            foreach (var predicate in sorted) {
                if (cp.CompareTo(predicate) == 0) {
                    newGroup.Add(predicate);
                } else {
                    cp = predicate;
                    outcomePatterns.Add(newGroup);
                    newGroup = new List<ComparablePredicate> { predicate };
                }
            }
            outcomePatterns.Add(newGroup);
            //System.err.println(outcomePatterns.size() + " outcome patterns");
            return outcomePatterns;
        }

        public override void Persist() {

            // the type of model (NaiveBayes)
            Write("NaiveBayes");

            // the mapping from outcomes to their integer indexes
            Write(OutcomeLabels.Length);

            foreach (var label in OutcomeLabels) {
                Write(label);
            }

            // the mapping from predicates to the outcomes they contributed to.
            // The sorting is done so that we actually can write this out more
            // compactly than as the entire list.
            var sorted = SortValues();
            var compressed = ComputeOutcomePatterns(sorted);

            Write(compressed.Count);

            foreach (var a in compressed)
                Write(a.Count + a[0].ToString());
            
            // the mapping from predicate names to their integer indexes
            Write(sorted.Length);

            foreach (var s in sorted)
                Write(s.Name);

            // write out the parameters
            foreach (var p in sorted.SelectMany(s => s.Parameters))
                Write(p);

            Close();
        }
    }
}