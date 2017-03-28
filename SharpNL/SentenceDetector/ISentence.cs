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
using SharpNL.Chunker;
using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Parser;
using SharpNL.Tokenize;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Represents a sentence.
    /// </summary>
    public interface ISentence {

        #region + Properties .

        #region . Tokens .
        /// <summary>
        /// Gets the sentence categories.
        /// </summary>
        /// <value>The sentence categories.</value>
        IReadOnlyList<ICategory> Categories { get; set; }
        #endregion

        #region . Chunks .
        /// <summary>
        /// Gets the sentence chunks.
        /// </summary>
        /// <value>The sentence chunks.</value>
        IReadOnlyList<IChunk> Chunks { get; set; }
        #endregion

        #region . Document .
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        IDocument Document { get; }
        #endregion

        #region . End .
        /// <summary>
        /// Gets the sentence end position.
        /// </summary>
        /// <value>The sentence end position.</value>
        int End { get; }
        #endregion

        #region . Entities .
        /// <summary>
        /// Gets the sentence entities.
        /// </summary>
        /// <value>The sentence entities.</value>
        IReadOnlyList<IEntity> Entities { get; set; }
        #endregion

        #region . Length .
        /// <summary>
        /// Gets the sentence length.
        /// </summary>
        /// <value>The sentence length.</value>
        int Length { get; }
        #endregion

        #region . Parse .
        /// <summary>
        /// Gets the parsed sentence.
        /// </summary>
        /// <value>The parsed sentence.</value>
        Parse Parse { get; set; }
        #endregion

        #region . Start .
        /// <summary>
        /// Gets the sentence start position.
        /// </summary>
        /// <value>The sentence start position.</value>
        int Start { get; }
        #endregion

        #region . Text .
        /// <summary>
        /// Gets the sentence text.
        /// </summary>
        /// <value>The sentence text.</value>
        string Text { get; }
        #endregion

        #region . TagProbability .
        /// <summary>
        /// Gets the part-of-speech tagging probability.
        /// </summary>
        /// <value>The part-of-speech tagging probability.</value>
        double TagProbability { get; set; }
        #endregion

        #region . Tokens .
        /// <summary>
        /// Gets the sentence tokens.
        /// </summary>
        /// <value>The sentence tokens.</value>
        IReadOnlyList<IToken> Tokens { get; set; }
        #endregion

        #endregion

    }
}