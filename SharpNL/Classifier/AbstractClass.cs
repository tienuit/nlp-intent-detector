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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using SharpNL.Extensions;

namespace SharpNL.Classifier {
    /// <summary>
    /// Represents a abstract classifier class.
    /// </summary>
    [DebuggerDisplay("Class ({Name})")]
    public abstract class AbstractClass<F> : IClass<F> {

        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractClass{T}"/> class with its name.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        protected AbstractClass(string name) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        #endregion

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of features in this class.
        /// </summary>
        /// <value>The number of features in this class.</value>
        public int Count => Features.Count;

        #endregion

        #region . Name .
        /// <summary>
        /// Gets the class name.
        /// </summary>
        /// <value>The class name.</value>
        public string Name { get; protected set; }
        #endregion

        #region . Features .
        private HashSet<F> featureList;
        /// <summary>
        /// Gets the class features.
        /// </summary>
        /// <value>The class features.</value>
        public HashSet<F> Features => featureList ?? (featureList = new HashSet<F>());

        #endregion

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified features to the <see cref="AbstractClass{T}"/> .
        /// </summary>
        /// <param name="features">The features to be added.</param>
        public void Add(params F[] features) {
            if (features == null)
                throw new ArgumentNullException(nameof(features));

            foreach (var feature in features)
                Features.Add(feature);

        }
        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether the class contains the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if the class contains the specified feature; otherwise, <c>false</c>.</returns>
        public virtual bool Contains(F feature) {
            return Features.Contains(feature);
        }
        #endregion

        #region + GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through the features.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the features.
        /// </returns>
        public IEnumerator<F> GetEnumerator() {
            return Features.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region + Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IClass<F> other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;

            return Name.Equals(other.Name) && Features.SequenceEqual(other.Features);
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
            if (obj.GetType() != GetType()) return false;
            return Equals((IClass<F>)obj);
        }

        #endregion

        #region . Tag . 
        /// <summary>
        /// Gets or sets the object that contains data about the class.
        /// </summary>
        /// <value>An <see cref="object"/> that contains data about the class. The default is <c>null</c>.</value>
        [DefaultValue(null)]
        public object Tag { get; set; }
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
                return
                    ((Name != null ? Name.GetHashCode() : 0) * 397) ^
                    (Features != null ? Features.GetHashCode() : 0);
            }
        }
        #endregion

    }
}