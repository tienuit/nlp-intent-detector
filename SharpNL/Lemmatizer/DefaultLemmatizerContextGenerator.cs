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

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Simple feature generator for learning statistical lemmatizers.
    /// </summary>
    /// <remarks>
    /// Features based on Grzegorz Chrupała. 2008. Towards a Machine-Learning
    /// Architecture for Lexical Functional Grammar Parsing.PhD dissertation,
    /// Dublin City University.
    /// </remarks>
    public class DefaultLemmatizerContextGenerator : ILemmatizerContextGenerator {
        private const int PrefixLength = 5;
        private const int SuffixLength = 7;

        /// <summary>
        /// Returns the contexts for lemmatizing of the specified index.
        /// </summary>
        /// <param name="index">The index of the token in the specified toks array for which the context should be constructed.</param>
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
        public string[] GetContext(int index, string[] tokens, string[] tags, string[] lemmas) {
            string w0; // Word
            string t0; // Tag
            string p_1; // Previous prediction

            var lex = tokens[index];
            if (index < 1) {
                p_1 = "p_1=bos";
            } else {
                p_1 = "p_1=" + lemmas[index - 1];
            }

            w0 = "w0=" + tokens[index];
            t0 = "t0=" + tags[index];

            var features = new List<string> {
                w0,
                t0,
                p_1,
                p_1 + t0,
                p_1 + w0
            };

            // do some basic suffix analysis
            features.AddRange(GetSuffixes(lex).Select(suffix => "suf=" + suffix));
            features.AddRange(GetPrefixes(lex).Select(prefix => "pre=" + prefix));

            // see if the word has any special characters
            if (lex.IndexOf('-') != -1)
                features.Add("h");

            if (lex.Any(char.IsUpper))
                features.Add("c");

            if (lex.Any(char.IsNumber))
                features.Add("d");

            return features.ToArray();
        }

        public string[] GetContext(int index, string[] sequence, string[] priorDecisions, object[] additionalContext) {
            return GetContext(index, sequence, (string[]) additionalContext[0], priorDecisions);
        }

        protected static string[] GetPrefixes(string lex) {
            var prefs = new string[PrefixLength];
            for (int li = 1, ll = PrefixLength; li < ll; li++)
                prefs[li] = lex.Substring(0, Math.Min(li + 1, lex.Length));

            return prefs;
        }

        protected static string[] GetSuffixes(string lex) {
            var suffs = new string[SuffixLength];
            for (int li = 1, ll = SuffixLength; li < ll; li++)
                suffs[li] = lex.Substring(Math.Max(lex.Length - li - 1, 0));

            return suffs;
        }
    }
}