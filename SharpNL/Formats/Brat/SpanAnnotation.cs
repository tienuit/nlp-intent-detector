﻿// 
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

using SharpNL.Utility;

namespace SharpNL.Formats.Brat {
    /// <summary>
    /// Represents a Brat span annotation.
    /// </summary>
    public class SpanAnnotation : BratAnnotation {
        internal SpanAnnotation(string id, string type, Span span, string coveredText) : base(id, type) {
            Span = span;
            CoveredText = coveredText;
        }

        /// <summary>
        /// Gets the span.
        /// </summary>
        /// <value>The span.</value>
        public Span Span { get; private set; }

        /// <summary>
        /// Gets the covered text.
        /// </summary>
        /// <value>The covered text.</value>
        public string CoveredText { get; private set; }

        /// <summary>
        /// Returns a string that represents the current annotation.
        /// </summary>
        /// <returns>
        /// A string that represents the current annotation.
        /// </returns>
        public override string ToString() {
            return $"{base.ToString()} {Span.Start} {Span.End} {CoveredText}";
        }
    }
}