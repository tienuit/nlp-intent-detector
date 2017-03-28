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

namespace SharpNL.Classifier {
    /// <summary>
    /// Represents a abstract class classifier result.
    /// </summary>
    /// <typeparam name="C">The class type.</typeparam>
    /// <typeparam name="F">The feature type.</typeparam>
    public abstract class AbstractResult<C, F> : IClassClassifierResult<C, F> 
        where C : IClass<F> {

        /// <summary>
        /// Gets the result class.
        /// </summary>
        /// <value>The result class.</value>
        public C Class { get; protected set; }

        /// <summary>
        /// Gets the result probability.
        /// </summary>
        /// <value>The result probability.</value>
        public double Probability { get; protected set; }

        #region . CompareTo .
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IClassClassifierResult<C, F> other) {
            return other.Probability.CompareTo(Probability);
        }
        #endregion

    }
}