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
    /// Rosenbrock function: 
    /// <code>
    /// f(x,y) = (1-x)^2 + 100*(y-x^2)^2
    /// f(x,y) is non-convex and has global minimum at (x,y) = (1,1) where f(x,y) = 0
    /// 
    /// f_x = -2*(1-x) - 400*(y-x^2)*x
    /// f_y = 200*(y-x^2)
    /// </code>
    /// </summary>
    /// <seealso href="http://en.wikipedia.org/wiki/Rosenbrock_function"/>
    public class RosenbrockFunction : IFunction {

        /// <summary>
        /// Gets the current dimension.
        /// </summary>
        /// <value>The current dimension.</value>
        public int Dimension => 2;

        /// <summary>
        /// Gets the rosenbrock function value at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The rosenbrock function value.</returns>
        public double ValueAt(double[] x) {
            return Math.Pow(1 - x[0], 2) + 100 * Math.Pow(x[1] - Math.Pow(x[0], 2), 2);
        }

        /// <summary>
        /// Gets the gradient at the given input vector.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The gradient value.</returns>
        public double[] GradientAt(double[] x) {
            var g = new double[2];
            g[0] = -2 * (1 - x[0]) - 400 * (x[1] - Math.Pow(x[0], 2)) * x[0];
            g[1] = 200 * (x[1] - Math.Pow(x[0], 2));
            return g;
        }
    }
}