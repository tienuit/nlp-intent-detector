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

using System.Collections.Generic;

namespace SharpNL.Utility {
    /// <summary>
    /// Provide the basic methods for stopword providers.
    /// </summary>
    public interface IStopwordProvider {

        /// <summary>
        /// Gets the a set with all the stopwords.
        /// </summary>
        /// <value>The set with all the stopwords.</value>
        HashSet<string> Stopwords { get; }

        /// <summary>
        /// Determines whether the specified word is a stopword.
        /// </summary>
        /// <param name="word">The word check.</param>
        /// <returns><c>true</c> if the specified word is a stopword; otherwise, <c>false</c>.</returns>
        bool IsStopword(string word);

    }
}