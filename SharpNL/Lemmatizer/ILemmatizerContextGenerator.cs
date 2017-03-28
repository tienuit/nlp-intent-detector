//  
//  Copyright 2016 Gustavo J Knuppe (https://github.com/knuppe)
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

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Interface for the context generator used for probabilistic lemmatizer.
    /// </summary>
    public interface ILemmatizerContextGenerator : IBeamSearchContextGenerator<string> {
        /// <summary>
        /// Returns the contexts for lemmatizing of the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the token in the specified <paramref name="tokens" /> array for which the context
        /// should be constructed.
        /// </param>
        /// <param name="tokens">
        /// The tokens of the sentence. The <c>ToString</c> methods of these objects should return the token
        /// text.
        /// </param>
        /// <param name="tags">The POS tags for the the specified tokens.</param>
        /// <param name="lemmas">
        /// The previous decisions made in the tagging of this sequence. Only indices less than
        /// <paramref name="index" /> will be examined.
        /// </param>
        /// <returns>An array of predictive contexts on which a model basis its decisions.</returns>
        string[] GetContext(int index, string[] tokens, string[] tags, string[] lemmas);
    }
}