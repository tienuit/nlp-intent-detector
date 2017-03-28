// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
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
namespace SharpNL.Formats.Brat {
    /// <summary>
    /// Represents a Brat attribute annotation.
    /// </summary>
    public class AttributeAnnotation : BratAnnotation {

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeAnnotation"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The attribute type.</param>
        /// <param name="attachedTo">The attached to.</param>
        /// <param name="value">The attribute value.</param>
        public AttributeAnnotation(string id, string type, string attachedTo, string value)
            : base(id, type) {

            AttachedTo = attachedTo;
            Value = value;
        }

        #region + Properties .

        #region . AttachedTo .
        /// <summary>
        /// Gets the attached to.
        /// </summary>
        /// <value>The attached to.</value>
        public string AttachedTo { get; private set; }
        #endregion

        #region . Value .
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <value>The attribute value.</value>
        public string Value { get; private set; }
        #endregion

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current annotation.
        /// </summary>
        /// <returns>
        /// A string that represents the current annotation.
        /// </returns>
        public override string ToString() {
            return base.ToString() + " " + AttachedTo + (string.IsNullOrEmpty(Value) ? " " + Value : string.Empty);
        }
        #endregion

    }
}
