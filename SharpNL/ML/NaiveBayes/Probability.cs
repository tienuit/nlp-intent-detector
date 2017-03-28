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
using System.Collections.Generic;
using System.Globalization;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Represents a probability for a label.
    /// </summary>
    /// <typeparam name="T">The label type.</typeparam>
    public class Probability<T> : IProbability, IEquatable<Probability<T>> {

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="Probability{T}"/> class.
        /// </summary>
        /// <param name="label">The probability label.</param>
        public Probability(T label) {
            Label = label;
        }
        #endregion

        #region + Properties .

        #region . Label .
        /// <summary>
        /// Gets the probability label.
        /// </summary>
        /// <value>The probability label.</value>
        public T Label { get; }
        #endregion

        #region . Log .
        /// <summary>
        /// Gets the log probability associated with a label.
        /// </summary>
        /// <value>The log probability associated with a label.</value>
        public virtual double Log => Math.Log(Value);

        #endregion

        #region . Value .
        /// <summary>
        /// Gets or sets the probability value.
        /// </summary>
        /// <value>The probability value.</value>
        public virtual double Value { get; private set; }
        #endregion

        #endregion

        #region + Equals .

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
            if (obj.GetType() != GetType()) return false;
            return Equals((Probability<T>)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Probability<T> other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Label, other.Label) && Value.Equals(other.Value);
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
                return (EqualityComparer<T>.Default.GetHashCode(Label)*397) ^ Value.GetHashCode();
            }
        }
        #endregion

        #region . AddIn .
        /// <summary>
        /// Compounds the existing probability mass on the label with the new probability passed in to the method.
        /// </summary>
        /// <param name="probability">The probability weight to add.</param>
        public virtual void AddIn(double probability) {
            Value *= probability;
        }
        #endregion

        #region + IsLarger .
        /// <summary>
        /// Determines whether the specified probability is larger than the current probability.
        /// </summary>
        /// <param name="probability">The probability to check.</param>
        /// <returns><c>true</c> if the specified probability is larger than the current probability; otherwise, <c>false</c>.</returns>
        public virtual bool IsLarger(double probability) {
            return Value < probability;
        }

        /// <summary>
        /// Determines whether the specified probability is larger than the current probability.
        /// </summary>
        /// <param name="probability">The probability to check.</param>
        /// <returns><c>true</c> if the specified probability is larger than the current probability; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">probability</exception>
        public virtual bool IsLarger(IProbability probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));

            return Value < probability.Value;
        }
        #endregion

        #region + Set .
        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public virtual void Set(double probability) {
            Value = probability;
        }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public virtual void Set(IProbability probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));

            Value = probability.Value;
        }
        #endregion

        #region + SetIfLarger .
        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public virtual void SetIfLarger(double probability) {
            if (Value < probability)
                Value = probability;
        }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public virtual void SetIfLarger(IProbability probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));
            
            if (Value < probability.Value)
                Value = probability.Value;
        }
        #endregion

        #region . SetLog .
        /// <summary>
        /// Assigns a log probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The log probability to assign.</param>
        public virtual void SetLog(double probability) {
            Value = Math.Exp(probability);
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return EqualityComparer<T>.Default.Equals(Label, default(T)) 
                ? Value.ToString(CultureInfo.InvariantCulture) 
                : string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Label, Value);
        }
        #endregion

    }
}