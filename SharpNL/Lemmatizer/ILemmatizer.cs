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

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents a lemmatizer.
    /// </summary>
    /// <remarks>
    /// The inflected form may correspond to several lemmas (e.g. "found" -> find, found) - the correct choice depends
    /// on the context.
    /// </remarks>
    public interface ILemmatizer {
        /// <summary>
        /// Returns the lemma of the specified word with the specified part-of-speech.
        /// </summary>
        /// <param name="tokens">An array of the tokens.</param>
        /// <param name="tags">An array of the POS tags.</param>
        /// <returns>
        /// An array of lemma classes for each token in the sequence.
        /// </returns>
        string[] Lemmatize(string[] tokens, string[] tags);
    }
}