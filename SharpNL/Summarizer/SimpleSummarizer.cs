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
using System.ComponentModel;
using System.Linq;
using System.Text;
using SharpNL.Utility;

namespace SharpNL.Summarizer {
    /// <summary>
    /// A very simple text summarizer. This class cannot be inherited.
    /// </summary>
    public sealed class SimpleSummarizer : AbstractSummarizer {

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSummarizer" /> class.
        /// </summary>
        /// <param name="stopwordProvider">The stopword provider.</param>
        /// <param name="method">The summarization method.</param>
        public SimpleSummarizer(IStopwordProvider stopwordProvider, SimpleSummarizerMethods method) : base(stopwordProvider) {
            NumberOfSentences = 5;

            Method = method;
        }

        #endregion

        #region . NumberOfSentences .
        /// <summary>
        /// Gets or sets the amount of sentences in the summarization output. The default value is 5.
        /// </summary>
        /// <value>The amount of sentences in the summarization output.</value>
        [DefaultValue(5)]
        public int NumberOfSentences { get; set; }
        #endregion

        #region . Method .
        /// <summary>
        /// Gets the summarization method.
        /// </summary>
        /// <value>The summarization method.</value>
        public SimpleSummarizerMethods Method { get; private set; }
        #endregion

        protected override string ProcessSummarization(IDocument document) {

            var frequent = GetMostFrequentWords(125, document);
            var sb = new StringBuilder();

            switch (Method) {
                case SimpleSummarizerMethods.FirstSentence:

                    var sl = new List<string>(NumberOfSentences);

                    foreach (var sentence in document.Sentences) {
                        if (string.IsNullOrEmpty(sentence.Text))
                            continue;

                        if (frequent.Any(word => sentence.Text.IndexOf(word, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) > 0))
                            sl.Add(sentence.Text);

                        if (sl.Count >= NumberOfSentences)
                            goto donefs;
                    }

            donefs:

                    if (sl.Count == 0)
                        return string.Empty; // impossible ?

                    foreach (var sentence in sl) {
                        sb.Append(sentence);
                        sb.Append(' ');
                    }

                    return sb.ToString(0, sb.Length - 1);

                case SimpleSummarizerMethods.FrequentWords:
                    var sd = new Dictionary<string, int>();

                    foreach (var sentence in document.Sentences) {
                        if (string.IsNullOrEmpty(sentence.Text))
                            continue;

                        var count = frequent.Count(word => sentence.Text.IndexOf(word, IgnoreCase 
                            ? StringComparison.OrdinalIgnoreCase 
                            : StringComparison.Ordinal) > 0);

                        if (count <= 0) 
                            continue;

                        if (sd.ContainsKey(sentence.Text))
                            sd[sentence.Text] += count;
                        else
                            sd[sentence.Text] = count;
                    }

                    var list = sd.ToList();

                    list.Sort((one, two) => two.Value.CompareTo(one.Value));

                    for (var i = 0; i < Math.Min(list.Count, NumberOfSentences); i++) {
                        sb.Append(list[i].Key);
                        sb.Append(' ');
                    }

                    return sb.ToString(0, sb.Length - 1);

                default:
                    throw new NotSupportedException("The specified summarization method is not supported.");
            }
        }
    }
}