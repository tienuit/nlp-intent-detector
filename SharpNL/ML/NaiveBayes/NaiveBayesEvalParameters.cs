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

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Encapsulates the parameters for the evaluation of a naive bayes classifier.
    /// </summary>
    public class NaiveBayesEvalParameters : EvalParameters {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesEvalParameters"/> which can be evaluated.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="numOutcomes">The number outcomes.</param>
        /// <param name="outcomeTotals">The outcome totals.</param>
        /// <param name="vocabulary">The vocabulary value.</param>
        public NaiveBayesEvalParameters(Context[] parameters, int numOutcomes, double[] outcomeTotals, long vocabulary)
            : base(parameters, 0, 0, numOutcomes) {

            OutcomeTotals = outcomeTotals;
            Vocabulary = vocabulary;
        }

        /// <summary>
        /// Gets the outcome totals.
        /// </summary>
        /// <value>The outcome totals.</value>
        public double[] OutcomeTotals { get; private set; }

        /// <summary>
        /// Gets the vocabulary.
        /// </summary>
        /// <value>The vocabulary.</value>
        public long Vocabulary { get; private set; }

    }
}