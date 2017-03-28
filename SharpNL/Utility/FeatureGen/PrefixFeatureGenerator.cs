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

using System;
using System.Collections.Generic;

namespace SharpNL.Utility.FeatureGen {
    [TypeClass("opennlp.tools.util.featuregen.PrefixFeatureGenerator")]
    public class PrefixFeatureGenerator : FeatureGeneratorAdapter {

        private const int PREFIX_LENGTH = 4;

        public static string[] GetPrefixes(string lex) {
            var prefixes = new string[PREFIX_LENGTH];
            for (int li = 0, ll = PREFIX_LENGTH; li < ll; li++) {
                prefixes[li] = lex.Substring(0, Math.Min(li + 1, lex.Length));
            }
            return prefixes;
        }

        /// <summary>
        /// Adds the appropriate features for the token at the specified index with the
        /// specified array of previous outcomes to the specified list of features.
        /// </summary>
        /// <param name="features">The list of features to be added to.</param>
        /// <param name="tokens">The tokens of the sentence or other text unit being processed.</param>
        /// <param name="index">The index of the token which is currently being processed.</param>
        /// <param name="previousOutcomes">The outcomes for the tokens prior to the specified index.</param>
        public override void CreateFeatures(List<string> features, string[] tokens, int index, string[] previousOutcomes) {
            var prefixes = GetPrefixes(tokens[index]);
            foreach (var prefix in prefixes) {
                features.Add("pref=" + prefix);
            }
        }
    }
}