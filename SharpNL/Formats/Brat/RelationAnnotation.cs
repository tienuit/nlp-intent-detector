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
    /// Represents a Brat relation annotation.
    /// </summary>
    public class RelationAnnotation : BratAnnotation {

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationAnnotation"/> class.
        /// </summary>
        /// <param name="id">The relation identifier.</param>
        /// <param name="type">The relation type.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public RelationAnnotation(string id, string type, string arg1, string arg2) : base(id, type) {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        /// <summary>
        /// Gets the arg1.
        /// </summary>
        /// <value>The arg1.</value>
        public string Arg1 { get; private set; }

        /// <summary>
        /// Gets the arg2.
        /// </summary>
        /// <value>The arg2.</value>
        public string Arg2 { get; private set; }

        /// <summary>
        /// Returns a string that represents the current annotation.
        /// </summary>
        /// <returns>
        /// A string that represents the current annotation.
        /// </returns>
        public override string ToString() {
            return $"{base.ToString()} arg1: {Arg1} arg2: {Arg2}";
        }
    }
}