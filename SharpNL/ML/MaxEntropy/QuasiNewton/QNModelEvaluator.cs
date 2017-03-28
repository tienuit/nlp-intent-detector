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
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// A model evaluator for measuring model's QN training accuracy.
    /// </summary>
    public class QNModelEvaluator : IEvaluator {

        /// <summary>
        /// The data indexer
        /// </summary>
        private readonly IDataIndexer indexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="QNModelEvaluator"/> class.
        /// </summary>
        /// <param name="indexer">The data indexer.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="indexer"/> is null.</exception>
        public QNModelEvaluator(IDataIndexer indexer) {
            if (indexer == null)
                throw new ArgumentNullException(nameof(indexer));

            this.indexer = indexer;

        }

        /// <summary>
        /// Measure quality of the training parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The evaluated result.</returns>
        public double Evaluate(double[] parameters) {
            var contexts = indexer.GetContexts();
            var values = indexer.Values;
            var nEventsSeen = indexer.GetNumTimesEventsSeen();
            var outcomeList = indexer.GetOutcomeList();
            var nOutcomes = outcomeList.Length;
            var nPredLabels = indexer.GetPredLabels().Length;

            var nCorrect = 0;
            var nTotalEvents = 0;

            for (var ei = 0; ei < contexts.Length; ei++) {
                var context = contexts[ei];
                var value = values == null ? null : values[ei];

                var probs = new double[nOutcomes];

                QNModel.Eval(context, value, probs, nOutcomes, nPredLabels, parameters);

                var outcome = ArrayMath.MaxId(probs);
                if (outcome == outcomeList[ei])
                    nCorrect += nEventsSeen[ei];
                
                nTotalEvents += nEventsSeen[ei];
            }

            return nTotalEvents == 0 ? 0 : (double) nCorrect/nTotalEvents;
        }
    }
}