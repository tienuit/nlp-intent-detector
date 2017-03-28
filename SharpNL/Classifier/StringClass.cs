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

namespace SharpNL.Classifier {
    /// <summary>
    /// Represents a string classifier class.
    /// </summary>
    public class StringClass : AbstractClass<string> {

        #region . Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="StringClass"/> class with its name ignoring the feature case in the comparison.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public StringClass(string name) : this(name, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringClass"/> class with its name and a value indicating to ignore the case in the feature comparison.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the feature comparison should ignore the case.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public StringClass(string name, bool ignoreCase) : base(name) {
            IgnoreCase = ignoreCase;
        }

        #endregion

        #region + Properties .
               
        #region . IgnoreCase .
        /// <summary>
        /// Gets or sets a value indicating whether the feature comparison should ignore the case.
        /// </summary>
        /// <value><c>true</c> if set to <c>true</c> the feature comparison should ignore the case.</value>
        public bool IgnoreCase { get; protected set; }
        #endregion

        #endregion
               
        #region . Contains .
        /// <summary>
        /// Determines whether the class contains the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if the class contains the specified feature; otherwise, <c>false</c>.</returns>
        public override bool Contains(string feature) {
            return Features.Any(f => string.Equals(f, feature, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }
        #endregion

    }
}