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
    /// Quadratic function: f(x,y) = (x-1)^2 + (y-5)^2 + 10
    /// </summary>
    public class QuadraticFunction : IFunction {
        /// <summary>
        /// Gets the current dimension.
        /// </summary>
        /// <value>The current dimension.</value>
        public int Dimension => 2;

        /// <summary>
        /// Gets the function value at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The function value.</returns>
        public double ValueAt(double[] x) {
            return Math.Pow(x[0] - 1, 2) + Math.Pow(x[1] - 5, 2) + 10;
        }

        /// <summary>
        /// Gets the gradient at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The gradient value.</returns>
        public double[] GradientAt(double[] x) {
            return new[] {
                2*(x[0] - 1),
                2*(x[1] - 5)
            };
        }
    }
}