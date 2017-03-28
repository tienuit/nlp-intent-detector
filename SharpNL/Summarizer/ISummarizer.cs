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
using SharpNL.Tokenize;
using SharpNL.SentenceDetector;

namespace SharpNL.Summarizer {
    /// <summary>
    /// Represents a text summarizer.
    /// </summary>
    public interface ISummarizer {

        /// <summary>
        /// Summarizes the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The summarized string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="document"/>
        /// </exception>
        string Summarize(IDocument document);

        /// <summary>
        /// Summarizes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="sentenceDetector">The sentence detector.</param>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <returns>The summarized string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sentenceDetector"/>
        /// or
        /// <paramref name="tokenizer"/>
        /// </exception>
        string Summarize(string input, ISentenceDetector sentenceDetector, ITokenizer tokenizer);

    }
}