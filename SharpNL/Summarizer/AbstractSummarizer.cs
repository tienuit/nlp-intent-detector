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
using SharpNL.Analyzer;
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Summarizer {

    /// <summary>
    /// Represents a abstract summarizer.
    /// </summary>
    public abstract class AbstractSummarizer : ISummarizer {

        private readonly IStopwordProvider stopwordProvider;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSummarizer"/> class.
        /// </summary>
        protected AbstractSummarizer() {
            IgnoreCase = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSummarizer"/> class with a specified stopword provider.
        /// </summary>
        /// <param name="stopwordProvider">The stopword provider.</param>
        protected AbstractSummarizer(IStopwordProvider stopwordProvider) : this() {
            this.stopwordProvider = stopwordProvider;
        }
        #endregion

        #region + Properties .

        #region . IgnoreCase .
        /// <summary>
        /// Gets or sets a value indicating whether the summarizer should ignore case. The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the summarizer should ignore case; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool IgnoreCase { get; set; }
        #endregion

        #endregion

        #region + Methods .

        #region . GetWordFrequency .
        /// <summary>
        /// Gets the word frequency in the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the comparison should ignore the case.</param>
        /// <returns>A dictionary containing each word and its frequency.</returns>
        protected Dictionary<string, int> GetWordFrequency(IDocument document, bool ignoreCase = true) {

            var dict = new Dictionary<string, int>(ignoreCase
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal);

            foreach (var sentence in document.Sentences) {
                foreach (var token in sentence.Tokens) {
                    if (string.IsNullOrEmpty(token.Lexeme) || IsStopword(token.Lexeme))
                        continue;

                    if (dict.ContainsKey(token.Lexeme))
                        dict[token.Lexeme]++;
                    else
                        dict[token.Lexeme] = 1;
                }
            }

            return dict;
        }
        #endregion GetWordFrequency

        #region . GetMostFrequentWords .

        protected HashSet<string> GetMostFrequentWords(int count, IDocument document, bool ignoreCase = true) {
            return GetMostFrequentWords(count, GetWordFrequency(document, ignoreCase));
        }

        protected HashSet<string> GetMostFrequentWords(int count,  Dictionary<string, int> frequencyDictionary, bool ignoreCase = true) {

            var set = new HashSet<string>(ignoreCase 
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal);

            var i = 0;
            foreach (var pair in frequencyDictionary.OrderByDescending(a => a.Value)) {               
                if (i++ > count)
                    break;

                set.Add(pair.Key);
            }
            return set;
        }

        #endregion

        #region . IsStopword .
        /// <summary>
        /// Determines whether the specified word is a stopword.
        /// </summary>
        /// <param name="word">The word check.</param>
        /// <returns><c>true</c> if the specified word is a stopword; otherwise, <c>false</c>.</returns>
        protected bool IsStopword(string word) {
            if (stopwordProvider == null || string.IsNullOrWhiteSpace(word))
                return false;

            return stopwordProvider.IsStopword(word);
        }
        #endregion

        #region . ProcessSummarization .
        /// <summary>
        /// Processes the summarization.
        /// </summary>
        /// <param name="document">The tokenized document.</param>
        /// <returns>The summarized string.</returns>
        protected abstract string ProcessSummarization(IDocument document);
        #endregion

        #region + Summarize .
        /// <summary>
        /// Summarizes the specified document.
        /// </summary>
        /// <param name="document">The document to be summarized.</param>
        /// <returns>The summarized string.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="document"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The specified <paramref name="document"/> does not have any sentence detected.
        /// </exception>
        public string Summarize(IDocument document) {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (document.Sentences == null || document.Sentences.Count == 0)
                throw new ArgumentException("The specified document does not have any sentence detected.", nameof(document));

            return ProcessSummarization(document);
        }

        /// <summary>
        /// Summarizes the specified input using the specified <paramref name="sentenceDetector"/> and <paramref name="tokenizer"/>.
        /// </summary>
        /// <param name="input">The input string to be summarized.</param>
        /// <param name="sentenceDetector">The sentence detector.</param>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <returns>The summarized string.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sentenceDetector"/>
        /// or
        /// <paramref name="tokenizer"/>
        /// </exception>
        public string Summarize(string input, ISentenceDetector sentenceDetector, ITokenizer tokenizer) {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (sentenceDetector == null)
                throw new ArgumentNullException(nameof(sentenceDetector));

            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer));

            var doc = new Document("x-unspecified", input);
            var anl = new AggregateAnalyzer {
                new SentenceDetectorAnalyzer(sentenceDetector),
                new TokenizerAnalyzer(tokenizer)
            };

            anl.Analyze(doc);

            return ProcessSummarization(doc);
        }
        #endregion

        #endregion Methods

    }
}