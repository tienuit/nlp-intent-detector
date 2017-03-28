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

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// Class that performs line search to find minimum.
    /// </summary>
    public class LineSearch {
        private static readonly double C = 0.0001;
        private static readonly double RHO = 0.5; // decrease of step size (must be from 0 to 1)

        /// <summary>
        /// Backtracking line search. (see Nocedal &amp; Wright 2006, Numerical Optimization, p. 37)
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="lsr">The result.</param>
        /// <param name="initialStepSize">Initial step size.</param>
        public static void DoLineSearch(
            IFunction function,
            double[] direction,
            LineSearchResult lsr,
            double initialStepSize) {
            var stepSize = initialStepSize;
            var currFctEvalCount = lsr.FctEvalCount;
            var x = lsr.NextPoint;
            var gradAtX = lsr.GradAtNext;
            var valueAtX = lsr.ValueAtNext;
            var dimension = x.Length;

            // Retrieve current points and gradient for array reuse purpose
            var nextPoint = lsr.CurrPoint;
            var gradAtNextPoint = lsr.GradAtCurr;
            double valueAtNextPoint;

            var dirGradientAtX = ArrayMath.InnerProduct(direction, gradAtX);

            // To avoid recomputing in the loop
            var cachedProd = C*dirGradientAtX;

            while (true) {

                // Get next point
                for (var i = 0; i < dimension; i++)
                    nextPoint[i] = x[i] + direction[i]*stepSize;

                // New value
                valueAtNextPoint = function.ValueAt(nextPoint);

                currFctEvalCount++;

                // Check Armijo condition
                if (valueAtNextPoint <= valueAtX + cachedProd*stepSize)
                    break;

                

                // Shrink step size
                stepSize *= RHO;
            }

            // Compute and save gradient at the new point
            Array.Copy(function.GradientAt(nextPoint), 0, gradAtNextPoint, 0, gradAtNextPoint.Length);

            // Update line search result
            lsr.SetAll(stepSize, valueAtX, valueAtNextPoint, gradAtX, gradAtNextPoint, x, nextPoint, currFctEvalCount);
        }

        /// <summary>
        /// Constrained line search (see section 3.2 in the paper "Scalable Training of L1-Regularized Log-Linear Models", Andrew et al. 2007)
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="lsr">The line search result.</param>
        /// <param name="l1Cost">The l1 cost.</param>
        /// <param name="initialStepSize">Initial size of the step.</param>
        public static void DoConstrainedLineSearch(
            IFunction function, 
            double[] direction, 
            LineSearchResult lsr,
            double l1Cost, 
            double initialStepSize) {


            var stepSize = initialStepSize;
            var currFctEvalCount = lsr.FctEvalCount;
            var x = lsr.NextPoint;
            var signX = lsr.SignVector; // existing sign vector
            var gradAtX = lsr.GradAtNext;
            var pseudoGradAtX = lsr.PseudoGradAtNext;
            var valueAtX = lsr.ValueAtNext;
            var dimension = x.Length;

            // Retrieve current points and gradient for array reuse purpose
            var nextPoint = lsr.CurrPoint;
            var gradAtNextPoint = lsr.GradAtCurr;
            double valueAtNextPoint;

            // New sign vector
            for (var i = 0; i < dimension; i++) {
                signX[i] = x[i].Equals(0d) ? -pseudoGradAtX[i] : x[i];
            }

            while (true) {
                // Get next point
                for (var i = 0; i < dimension; i++) {
                    nextPoint[i] = x[i] + direction[i]*stepSize;
                }

                // Projection
                for (var i = 0; i < dimension; i++) {
                    if (nextPoint[i]*signX[i] <= 0)
                        nextPoint[i] = 0;
                }

                // New value
                valueAtNextPoint = function.ValueAt(nextPoint) + l1Cost*ArrayMath.L1Norm(nextPoint);

                currFctEvalCount++;

                double dirGradientAtX = 0;
                for (var i = 0; i < dimension; i++) {
                    dirGradientAtX += (nextPoint[i] - x[i])*pseudoGradAtX[i];
                }

                // Check the sufficient decrease condition
                if (valueAtNextPoint <= valueAtX + C*dirGradientAtX)
                    break;

                // Shrink step size
                stepSize *= RHO;
            }

            // Compute and save gradient at the new point
            Array.Copy(function.GradientAt(nextPoint), 0, gradAtNextPoint, 0, gradAtNextPoint.Length);

            // Update line search result
            lsr.SetAll(stepSize, valueAtX, valueAtNextPoint, gradAtX, gradAtNextPoint, pseudoGradAtX, x, nextPoint,
                signX, currFctEvalCount);
        }
    }
}