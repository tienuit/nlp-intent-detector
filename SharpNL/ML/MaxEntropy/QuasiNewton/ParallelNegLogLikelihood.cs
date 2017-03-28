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
using System.Threading.Tasks;
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    public class ParallelNegLogLikelihood : NegLogLikelihood {
        // 
        /// <summary>
        /// The partial gradient
        /// </summary>
        private readonly double[][] gradientThread;

        /// <summary>
        /// Partial value of negative log-likelihood to be computed by each thread
        /// </summary>
        private readonly double[] negLogLikelihoodThread;

        /// <summary>
        /// The number of threads
        /// </summary>
        private readonly int threads;

        /// <summary>
        /// Initializes a new instance of the <see cref="NegLogLikelihood"/> class.
        /// </summary>
        /// <param name="indexer">The data indexer.</param>
        /// <param name="threads">The number of threads.</param>
        public ParallelNegLogLikelihood(IDataIndexer indexer, int threads) : base(indexer) {
            if (threads <= 0)
                throw new ArgumentOutOfRangeException(nameof(threads), "The number of threads must be 1 or larger.");

            this.threads = threads;

            negLogLikelihoodThread = new double[threads];
            gradientThread = new double[threads][];

            for (var i = 0; i < threads; i++)
                gradientThread[i] = new double[Dimension];
        }

        /// <summary>
        /// Gets the negative log-likelihood at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The negative log-likelihood.</returns>
        /// <exception cref="ArgumentException">The <paramref name="x"/> is invalid, its dimension is not equal to domain dimension.</exception>
        public override double ValueAt(double[] x) {
            if (x.Length != dimension) {
                throw new ArgumentException(
                    "The input vector is invalid, its dimension is not equal to domain dimension.");
            }


            var taskSize = numContexts/threads;
            var leftOver = numContexts%threads;

            var tasks = new Task[threads];

            for (var i = 0; i < threads; i++) {
                var id = i;
                if (id != threads - 1) {
                    tasks[id] = new Task(() => NegLLCompute(id, id*taskSize, taskSize, x));
                } else {
                    tasks[id] = new Task(() => NegLLCompute(id, id*taskSize, taskSize + leftOver, x));
                }

                tasks[id].Start();
            }

            Task.WaitAll(tasks);


            double negLogLikelihood = 0;
            for (var t = 0; t < threads; t++) {
                negLogLikelihood += negLogLikelihoodThread[t];
            }

            return negLogLikelihood;
        }


        /// <summary>
        /// Gets the gradient at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The gradient value.</returns>
        /// <exception cref="System.ArgumentException">x is invalid, its dimension is not equal to domain dimension.;x</exception>
        /// <exception cref="ArgumentException">The <paramref name="x" /> is invalid, its dimension is not equal to domain dimension.</exception>
        public override double[] GradientAt(double[] x) {
            var taskSize = numContexts / threads;
            var leftOver = numContexts % threads;

            var tasks = new Task[threads];

            for (var i = 0; i < threads; i++) {
                var id = i;

                if (id != threads - 1) {
                    tasks[id] = new Task(() => GradientCompute(id, id*taskSize, taskSize, x));
                } else {
                    tasks[id] = new Task(() => GradientCompute(id, id*taskSize, taskSize + leftOver, x));
                }

                tasks[id].Start();
            }

            Task.WaitAll(tasks);


            for (var i = 0; i < dimension; i++) {
                gradient[i] = 0;

                for (var t = 0; t < threads; t++) {
                    gradient[i] += gradientThread[t][i];
                }
            }

            return gradient;
        }

        private void NegLLCompute(int threadIndex, int startIndex, int length, double[] x) {
            negLogLikelihoodThread[threadIndex] = 0;

            // Knuppe: In parallel we can't use the tempSums variable ;)
            var temp = new double[numOutcomes];

            for (var ci = startIndex; ci < startIndex + length; ci++) {
                for (var oi = 0; oi < numOutcomes; oi++) {
                    temp[oi] = 0;
                    for (var ai = 0; ai < contexts[ci].Length; ai++) {
                        var vectorIndex = IndexOf(oi, contexts[ci][ai]);
                        var predValue = values != null ? values[ci][ai] : 1.0;
                        temp[oi] += predValue*x[vectorIndex];
                    }
                }

                var logSumOfExps = ArrayMath.LogSumOfExps(temp);

                var outcome = outcomeList[ci];

                negLogLikelihoodThread[threadIndex] -= (temp[outcome] - logSumOfExps)*numTimesEventsSeen[ci];
            }
        }



        private void GradientCompute(int threadIndex, int startIndex, int length, double[] x) {
            var exp = new double[numOutcomes];

            // Reset gradientThread
            Array.Clear(gradientThread[threadIndex], 0, gradientThread[threadIndex].Length);

            for (var ci = startIndex; ci < startIndex + length; ci++) {
                double predValue;
                int vectorIndex;
                for (var oi = 0; oi < numOutcomes; oi++) {
                    exp[oi] = 0;
                    for (var ai = 0; ai < contexts[ci].Length; ai++) {
                        vectorIndex = IndexOf(oi, contexts[ci][ai]);
                        predValue = values != null ? values[ci][ai] : 1.0;
                        exp[oi] += predValue*x[vectorIndex];
                    }
                }

                var logSumOfExps = ArrayMath.LogSumOfExps(exp);

                for (var oi = 0; oi < numOutcomes; oi++) {
                    exp[oi] = Math.Exp(exp[oi] - logSumOfExps);
                }

                for (var oi = 0; oi < numOutcomes; oi++) {
                    var empirical = outcomeList[ci] == oi ? 1 : 0;
                    for (var ai = 0; ai < contexts[ci].Length; ai++) {
                        vectorIndex = IndexOf(oi, contexts[ci][ai]);
                        predValue = values != null ? values[ci][ai] : 1.0;
                        gradientThread[threadIndex][vectorIndex] += predValue*(exp[oi] - empirical)*
                                                                    numTimesEventsSeen[ci];
                    }
                }
            }
        }
    }
}