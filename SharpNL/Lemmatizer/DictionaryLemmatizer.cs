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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents a dictionary based lemmatizer. This class cannot be inherited.
    /// </summary>
    public sealed class DictionaryLemmatizer : ILemmatizer {
        private readonly Dictionary<string, string> dict;

        public DictionaryLemmatizer() {
            dict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Construct the dictionary from the input tab separated dictionary. The input file should have, for each line:
        /// word[tab]lemma[tab]postag
        /// </summary>
        /// <param name="dictionaryFile">The input dictionary file.</param>
        public DictionaryLemmatizer(string dictionaryFile) : this() {
            using (var reader = new StreamReader(dictionaryFile, Encoding.UTF8)) {
                for (var line = reader.ReadLine(); !string.IsNullOrEmpty(line); line = reader.ReadLine()) {
                    var parts = line.Split('\t');
                    if (parts.Length != 3)
                        continue; // ignore invalid line

                    dict[Key(parts[0], parts[2])] = parts[1];
                }
            }
        }

        /// <summary>
        /// Returns the lemma of the specified word with the specified part-of-speech.
        /// </summary>
        /// <param name="tokens">An array of the tokens.</param>
        /// <param name="tags">An array of the POS tags.</param>
        /// <returns>An array of lemma classes for each token in the sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tokens" /> or <paramref name="tags" /></exception>
        /// <exception cref="ArgumentException">The arguments must have the same length.</exception>
        public string[] Lemmatize(string[] tokens, string[] tags) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            if (tokens.Length != tags.Length)
                throw new ArgumentException("The arguments must have the same length.");

            var lemmas = new string[tokens.Length];
            string value;
            for (var i = 0; i < tokens.Length; i++)
                lemmas[i] = dict.TryGetValue(Key(tokens[i], tags[i]), out value) ? value : "O";

            return lemmas;
        }

        private static string Key(string word, string tag) {
            return $"{word}\u262f{tag}";
        }
    }
}