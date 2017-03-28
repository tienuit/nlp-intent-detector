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
    /// L2-regularized objective function.
    /// </summary>
    public class L2RegFunction : IFunction {

        private readonly IFunction func;
        private readonly double l2Cost;

        /// <summary>
        /// Initializes a new instance of the <see cref="L2RegFunction"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <param name="l2Cost">The l2 cost.</param>
        public L2RegFunction(IFunction func, double l2Cost) {
            this.func = func;
            this.l2Cost = l2Cost;
        }

        #region . Dimension .
        /// <summary>
        /// Gets the current dimension.
        /// </summary>
        /// <value>The current dimension.</value>
        public int Dimension => func.Dimension;

        #endregion


        /// <summary>
        /// Gets the function value at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The function value.</returns>
        public double ValueAt(double[] x) {
            CheckDimension(x);
            var value = func.ValueAt(x);
            if (l2Cost > 0) {
                value += l2Cost * ArrayMath.InnerProduct(x, x);
            }
            return value;
        }

        /// <summary>
        /// Gets the gradient at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The gradient value.</returns>
        public double[] GradientAt(double[] x) {
            CheckDimension(x);
            var gradient = func.GradientAt(x);
            if (l2Cost <= 0) 
                return gradient;

            for (var i = 0; i < x.Length; i++)
                gradient[i] += 2 * l2Cost * x[i];
            
            return gradient;
        }

        private void CheckDimension(double[] x) {
            if (x.Length != Dimension)
                throw new ArgumentOutOfRangeException(nameof(x), "x's dimension is not the same as function's dimension.");
        }

    }
}