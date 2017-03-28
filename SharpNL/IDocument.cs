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

using System.Collections.Generic;
using SharpNL.SentenceDetector;

namespace SharpNL {
    /// <summary>
    /// Represents a text document which can be several sentences, a sentence or even a single word.
    /// </summary>
    public interface IDocument {
        /// <summary>
        /// Gets the document text.
        /// </summary>
        /// <value>The document text.</value>
        string Text { get; }

        /// <summary>
        /// Gets the language of this document.
        /// </summary>
        /// <value>The language of this document.</value>
        string Language { get; }

        /// <summary>
        /// Gets the document sentences.
        /// </summary>
        /// <value>The document sentences.</value>
        IReadOnlyList<ISentence> Sentences { get; set; }

        /// <summary>
        /// Gets the factory associated to this document.
        /// </summary>
        /// <value>The factory associated to this document.</value>
        ITextFactory Factory { get; }

    }
}