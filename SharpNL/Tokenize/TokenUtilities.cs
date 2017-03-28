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
using System.Collections.Generic;
using System.Linq;

namespace SharpNL.Tokenize {

    /// <summary>
    /// Utilities for working with tokens.
    /// </summary>
    public static class TokenUtilities {

        #region . GetTokenCount .
        /// <summary>
        /// Gets the number of times a token appears in the specified token array.
        /// </summary>
        /// <param name="token">The token to count.</param>
        /// <param name="tokens">The tokens to evaluate.</param>
        /// <param name="ignoreCase">if set to <c>true</c> to indicate if the string comparison should ignore the case of the token.</param>
        /// <returns>The number of times a token appears in the specified token array.</returns>
        public static int GetTokenCount(string token, string[] tokens, bool ignoreCase) {
            return tokens.Count(item => string.Compare(token, item, ignoreCase) == 0);
        }

        #endregion GetTokenCount

        #region . GetTokenFrequency .
        /// <summary>
        /// Gets the token frequency.
        /// </summary>
        /// <param name="tokens">The tokens to evaluate.</param>
        /// <param name="ignoreCase">if set to <c>true</c> to indicate if the string comparison should ignore the case of the token.</param>
        /// <returns>A dictionary with the token frequency.</returns>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        public static Dictionary<string, int> GetTokenFrequency(string[] tokens, bool ignoreCase) {
          
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            var dict = new Dictionary<string, int>(ignoreCase 
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal);

            foreach (var token in tokens) {
                if (dict.ContainsKey(token))
                    dict[token]++;
                else
                    dict[token] = 1;
            }

            return dict;
        }

        #endregion GetTokenFrequency

        #region . GetUniqueTokens .
        /// <summary>
        /// Gets the unique tokens.
        /// </summary>
        /// <param name="tokens">The tokens to evaluate.</param>
        /// <param name="ignoreCase">if set to <c>true</c> to indicate if the string comparison should ignore the case of the token.</param>
        /// <returns>A set of unique tokens.</returns>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        public static HashSet<string> GetUniqueTokens(string[] tokens, bool ignoreCase) {

            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            var set = new HashSet<string>(ignoreCase 
                ? StringComparer.OrdinalIgnoreCase 
                : StringComparer.Ordinal);

            foreach (var token in tokens)
                set.Add(token);

            return set;
        }
        #endregion GetUniqueTokens

    }
}