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

using System.Diagnostics;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a string token.
    /// </summary>
    [DebuggerDisplay("StringToken({Kind}) - Line {Line}, Col {Column}")]
    public class StringToken {

        #region + Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="StringToken"/> class.
        /// </summary>
        /// <param name="kind">The token kind.</param>
        /// <param name="value">The token value.</param>
        /// <param name="line">The token line.</param>
        /// <param name="column">The token column.</param>
        public StringToken(StringTokenKind kind, string value, int line, int column) {
            Kind = kind;
            Value = value;
            Line = line;
            Column = column;
        }

        #endregion

        #region + Properties .

        #region . Column .

        /// <summary>
        /// Gets the token column.
        /// </summary>
        /// <value>The token column.</value>
        public int Column { get; protected set; }

        #endregion

        #region . Kind .

        /// <summary>
        /// Gets the token kind.
        /// </summary>
        /// <value>The token kind.</value>
        public StringTokenKind Kind { get; protected set; }

        #endregion

        #region . Line .

        /// <summary>
        /// Gets the token line.
        /// </summary>
        /// <value>The token line.</value>
        public int Line { get; protected set; }

        #endregion

        #region . Value .

        /// <summary>
        /// Gets the token value.
        /// </summary>
        /// <value>The token value.</value>
        public string Value { get; protected set; }

        #endregion

        #endregion
    }
}