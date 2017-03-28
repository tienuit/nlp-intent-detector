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
using System.Linq;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// Utility class for simple vector arithmetic.
    /// </summary>
    public static class ArrayMath {

        /// <summary>
        /// Calculates the product..
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The product. <c>SUM(a[n] * b[n])</c></returns>
        public static double InnerProduct(double[] a, double[] b) {
            if (a == null || b == null || a.Length != b.Length)
                return double.NaN;

            return a.Select((t, i) => t*b[i]).Sum();
        }


        /// <summary>
        /// L1-norm.
        /// </summary>
        /// <param name="x">The values normalize.</param>
        /// <returns>The normalized value.</returns>
        public static double L1Norm(double[] x) {
            return x.Sum(v => Math.Abs(v));
        }

        /// <summary>
        /// L2-norm.
        /// </summary>
        /// <param name="x">The values normalize.</param>
        /// <returns>The normalized value.</returns>
        public static double L2Norm(double[] x) {
            return Math.Sqrt(InnerProduct(x, x));
        }

        /// <summary>
        /// Inverse L2-norm
        /// </summary>
        /// <param name="x">The values normalize.</param>
        /// <returns>The inversed normalized value.</returns>
        public static double InvL2Norm(double[] x) {
            return 1/L2Norm(x);
        }

        /// <summary>
        /// Computes <c>log(sum_{i=1}^n e^{x_i})</c> using a maximum-element trick to avoid arithmetic overflow.
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>log-sum of exponentials of vector elements.</returns>
        public static double LogSumOfExps(double[] x) {
            var max = x.Max();
            var sum = x.Where(t => !double.IsNegativeInfinity(t)).Sum(t => Math.Exp(t - max));

            return max + Math.Log(sum);
        }


        /// <summary>
        /// Find index of maximum element in the vector x
        /// </summary>
        /// <param name="x">The input vector.</param>
        /// <returns>The index of the maximum element. Index of the first maximum element is returned if multiple maximums are found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">x</exception>
        public static int MaxId(double[] x) {
            if (x == null || x.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(x));

            var id = 0;

            for (var i = 0; i < x.Length; i++)
                if (x[id] < x[i]) id = i;

            return id;
        }



    }
}