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
    /// Evaluate negative log-likelihood and its gradient from DataIndexer.
    /// </summary>
    public class NegLogLikelihood : IFunction {
        protected readonly int[][] contexts;
        protected readonly int[] numTimesEventsSeen;
        protected readonly int[] outcomeList;
        /// <summary>
        /// Information from data index
        /// </summary>
        protected readonly float[][] values;
        protected int dimension;
        protected double[] expectation;
        protected double[] gradient;
        protected int numContexts;
        protected int numFeatures;
        protected int numOutcomes;

        /// <summary>
        /// For calculating negLogLikelihood and gradient
        /// </summary>
        protected double[] tempSums;

        /// <summary>
        /// Initializes a new instance of the <see cref="NegLogLikelihood"/> class.
        /// </summary>
        /// <param name="indexer">The data indexer.</param>
        public NegLogLikelihood(IDataIndexer indexer) {

            if (indexer == null)
                throw new ArgumentNullException(nameof(indexer));

            if (!indexer.Completed)
                indexer.Execute();

            values = indexer is OnePassRealValueDataIndexer ? indexer.Values : null;

            contexts = indexer.GetContexts();
            outcomeList = indexer.GetOutcomeList();
            numTimesEventsSeen = indexer.GetNumTimesEventsSeen();

            numOutcomes = indexer.GetOutcomeLabels().Length;
            numFeatures = indexer.GetPredLabels().Length;
            numContexts = contexts.Length;
            dimension = numOutcomes*numFeatures;

            expectation = new double[numOutcomes];
            tempSums = new double[numOutcomes];
            gradient = new double[dimension];
        }

        #region + Properties .

        #region . Dimension .
        /// <summary>
        /// Gets the dimension.
        /// </summary>
        /// <value>The dimension.</value>
        public int Dimension => dimension;

        #endregion

        #endregion

        #region . GetInitialPoint .
        /// <summary>
        /// Gets the initial point.
        /// </summary>
        /// <returns>A new array with the size of <see cref="P:Dimesion"/>.</returns>
        public double[] GetInitialPoint() {
            return new double[dimension];
        }
        #endregion

        #region . ValueAt .
        /// <summary>
        /// Gets the negative log-likelihood at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The negative log-likelihood.</returns>
        /// <exception cref="ArgumentException">The <paramref name="x"/> is invalid, its dimension is not equal to domain dimension.</exception>
        public virtual double ValueAt(double[] x) {
            if (x.Length != dimension)
                throw new ArgumentException("x is invalid, its dimension is not equal to domain dimension.", nameof(x));

            int ci;
            double negLogLikelihood = 0;

            for (ci = 0; ci < numContexts; ci++) {
                int oi;
                for (oi = 0; oi < numOutcomes; oi++) {
                    tempSums[oi] = 0;
                    int ai;
                    for (ai = 0; ai < contexts[ci].Length; ai++) {
                        var vectorIndex = IndexOf(oi, contexts[ci][ai]);
                        var predValue = values != null ? values[ci][ai] : 1.0;
                        tempSums[oi] += predValue * x[vectorIndex];
                    }
                }

                var logSumOfExps = ArrayMath.LogSumOfExps(tempSums);

                negLogLikelihood -= (tempSums[outcomeList[ci]] - logSumOfExps) * numTimesEventsSeen[ci];
            }

            return negLogLikelihood;


        }
        #endregion

        #region . GradientAt .
         /// <summary>
        /// Gets the gradient at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The gradient value.</returns>
        /// <exception cref="System.ArgumentException">x is invalid, its dimension is not equal to domain dimension.;x</exception>
        /// <exception cref="ArgumentException">The <paramref name="x" /> is invalid, its dimension is not equal to domain dimension.</exception>
        public virtual double[] GradientAt(double[] x) {
            if (x.Length != dimension)
                throw new ArgumentException("x is invalid, its dimension is not equal to domain dimension.", nameof(x));

            int ci;

            // Reset gradient
            for (var i = 0; i < gradient.Length; i++)
                gradient[i] = 0;

            for (ci = 0; ci < numContexts; ci++) {
                int oi;
                double predValue;
                int vectorIndex;
                int ai;
                for (oi = 0; oi < numOutcomes; oi++) {
                    expectation[oi] = 0;
                    for (ai = 0; ai < contexts[ci].Length; ai++) {
                        vectorIndex = IndexOf(oi, contexts[ci][ai]);
                        predValue = values != null ? values[ci][ai] : 1.0;
                        expectation[oi] += predValue * x[vectorIndex];
                    }
                }

                var logSumOfExps = ArrayMath.LogSumOfExps(expectation);

                for (oi = 0; oi < numOutcomes; oi++)
                    expectation[oi] = Math.Exp(expectation[oi] - logSumOfExps);

                for (oi = 0; oi < numOutcomes; oi++) {
                    var empirical = outcomeList[ci] == oi ? 1 : 0;
                    for (ai = 0; ai < contexts[ci].Length; ai++) {
                        vectorIndex = IndexOf(oi, contexts[ci][ai]);
                        predValue = values != null ? values[ci][ai] : 1.0;
                        gradient[vectorIndex] +=
                            predValue * (expectation[oi] - empirical) * numTimesEventsSeen[ci];
                    }
                }
            }

            return gradient;
        }
        #endregion

        #region . IndexOf .
        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="outcomeId">The outcome identifier.</param>
        /// <param name="featureId">The feature identifier.</param>
        /// <returns>System.Int32.</returns>
        protected int IndexOf(int outcomeId, int featureId) {
            return outcomeId*numFeatures + featureId;
        }
        #endregion

    }
}