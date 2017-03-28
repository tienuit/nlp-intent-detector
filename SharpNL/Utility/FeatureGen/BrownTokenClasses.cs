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

namespace SharpNL.Utility.FeatureGen {

    /// <summary>
    /// Obtain the paths listed in the pathLengths array from the Brown class.
    /// </summary>
    internal static class BrownTokenClasses {

        internal static readonly int[] pathLengths = {4, 6, 10, 20};

        /// <summary>
        /// It provides a list containing the pathLengths for a token if found in the <see cref="BrownCluster"/>.
        /// </summary>
        /// <param name="token">The token to be looked up in the brown clustering map.</param>
        /// <param name="brownLexicon">The Brown clustering map.</param>
        /// <returns>The list of the paths for a token.</returns>
        public static List<string> GetWordClasses(string token, BrownCluster brownLexicon) {
            if (brownLexicon[token] == null)
                return new List<string>();

            var brownClass = brownLexicon[token];

            var pathLengthsList = new List<string> {
                brownClass.Substring(0, Math.Min(brownClass.Length, pathLengths[0]))
            };

            for (var i = 1; i < pathLengths.Length; i++) {
                if (pathLengths[i - 1] < brownClass.Length) {
                    pathLengthsList.Add(brownClass.Substring(0, Math.Min(brownClass.Length, pathLengths[i])));
                }
            }
            return pathLengthsList;
        }
    }
}