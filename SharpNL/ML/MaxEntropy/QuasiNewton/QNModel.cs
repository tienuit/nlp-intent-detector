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
using SharpNL.Extensions;
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// A maximum entropy model which has been trained using the quasi-Newton optimization procedure.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Quasi-Newton_method"/>
    public class QNModel : AbstractModel, IEquatable<QNModel> {

        /// <summary>
        /// Creates a new model with the specified parameters, outcome names, and predicate/feature labels.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="predLabels">The names of the predicates used in this model.</param>
        /// <param name="outcomeNames">The names of the outcomes this model predicts.</param>
        public QNModel(Context[] parameters, string[] predLabels, string[] outcomeNames) : base(parameters, predLabels, outcomeNames) {
            ModelType = ModelType.MaxentQn;
        }

        #region + Eval .

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
        /// <param name="values">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, float[] values) {
            return Eval(context, values, new double[evalParameters.NumOutcomes]);
        }

        /// <summary>
        /// Model evaluation which should be used during inference.
        /// </summary>
        /// <param name="context">The predicates which have been observed at the present decision point.</param>
        /// <param name="values">The weights of the predicates which have been observed at the present decision point.</param>
        /// <param name="probs">The probability for outcomes.</param>
        /// <returns>The normalized probabilities for the outcomes given the context.</returns>
        public double[] Eval(string[] context, float[] values, double[] probs) {

            var ep = evalParameters.Parameters;

            for (var ci = 0; ci < context.Length; ci++) {
                var predIdx = GetPredIndex(context[ci]);

                if (predIdx < 0) 
                    continue;

                var predValue = 1d;

                if (values != null) 
                    predValue = values[ci];
                
                var outcomes = ep[predIdx].Outcomes;
                var parameters = ep[predIdx].Parameters;
                for (var i = 0; i < outcomes.Length; i++)
                    probs[outcomes[i]] += predValue*parameters[i];
                
            }

            var logSumExp = ArrayMath.LogSumOfExps(probs);
            for (var oi = 0; oi < outcomeNames.Length; oi++)
                probs[oi] = Math.Exp(probs[oi] - logSumExp);
            
            return probs;
        }

        /// <summary>
        /// Model evaluation which should be used during training to report model accuracy.
        /// </summary>
        /// <param name="context">Indices of the predicates which have been observed at the present decision point.</param>
        /// <param name="values">The weights of the predicates which have been observed at the present decision point.</param>
        /// <param name="probs">The probability for outcomes.</param>
        /// <param name="nOutcomes">The number of outcomes.</param>
        /// <param name="nPredLabels">The number of unique predicates.</param>
        /// <param name="parameters">The model parameters.</param>
        /// <returns>The normalized probabilities for the outcomes given the context.</returns>
        public static double[] Eval(int[] context, float[] values, double[] probs, int nOutcomes, int nPredLabels, double[] parameters) {
            for (var i = 0; i < context.Length; i++) {
                var predIdx = context[i];
                var predValue = values != null ? values[i] : 1d;

                for (var oi = 0; oi < nOutcomes; oi++)
                    probs[oi] += predValue * parameters[oi * nPredLabels + predIdx];
            }

            var logSumExp = ArrayMath.LogSumOfExps(probs);

            for (var oi = 0; oi < nOutcomes; oi++)
                probs[oi] = Math.Exp(probs[oi] - logSumExp);
            

            return probs;
        }

        #endregion

        #region + Equals .

        /// <summary>
        /// Indicates whether the current model is equal to another model.
        /// </summary>
        /// <returns>
        /// true if the current model is equal to the <paramref name="other"/> model; otherwise, false.
        /// </returns>
        /// <param name="other">An model to compare with this model.</param>
        public bool Equals(QNModel other) {
            if (other == null)
                return false;

            if (!outcomeNames.SequenceEqual(other.outcomeNames))
                return false;

            if (!map.Equals(other.map))
                return false;

            // compare parameters
            var ep = evalParameters.Parameters;
            var op = other.evalParameters.Parameters;
            if (ep.Length != op.Length)
                return false;

            for (var p = 0; p < ep.Length; p++) {

                if (!ep[p].Outcomes.SequenceEqual(op[p].Outcomes))
                    return false;

                if (!ep[p].Parameters.SequenceEqual(op[p].Parameters))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as QNModel);
        }

        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = outcomeNames.Length.GetHashCode();
                hashCode = (hashCode * 397) ^ map.GetHashCode();
                hashCode = (hashCode * 397) ^ evalParameters.GetHashCode();
                return hashCode;
            }
        }
        #endregion

    }
}