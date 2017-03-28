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

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a real valued parameter or expected value associated with a particular contextual predicate or feature. 
    /// This is used to store maxent model parameters as well as model and empirical expected values.
    /// </summary>
    public class Context : IEquatable<Context> {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> with the specified parameters associated with the specified outcome pattern.
        /// </summary>
        /// <param name="outcomes">The outcomes outcomes for which parameters exists for this context..</param>
        /// <param name="parameters">The parameters for the outcomes specified.</param>
        public Context(int[] outcomes, double[] parameters) {
            Outcomes = outcomes;
            Parameters = parameters;
        }

        #region + Properties .

        #region . Outcomes .
        /// <summary>
        /// Gets the outcomes for which parameters exists for this context.
        /// </summary>
        /// <value>A array of outcomes for which parameters exists for this context.</value>
        public int[] Outcomes { get; protected set; }
        #endregion

        #region . Parameters .
        /// <summary>
        /// Gets the parameters or expected values for the outcomes which occur with this context.
        /// </summary>
        /// <value>A array of parameters for the outcomes of this context.</value>
        public double[] Parameters { get; protected set; }
        #endregion

        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether this context contains the specified outcome.
        /// </summary>
        /// <param name="outcome">The outcome to seek.</param>
        /// <returns><c>true</c> if the <paramref name="outcome"/> occurs within this context; otherwise, <c>false</c>.</returns>
        public bool Contains(int outcome) {
            return Array.BinarySearch(Outcomes, outcome) >= 0;
        }
        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:Context"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return ((Outcomes?.GetArrayHash() ?? 0)*397) ^
                       (Parameters?.GetArrayHash() ?? 0);
            }
        }

        #endregion

        #region . Equals .

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Context other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!Outcomes.SequenceEqual(other.Outcomes))
                return false;

            if (!Parameters.SequenceEqual(other.Parameters))
                return false;

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
            return Equals(obj as Context);
        }

        #endregion
    }
}