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
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Convert {
    /// <summary>
    /// Represents a abstract sentence sample stream converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractToSentenceSampleStream<T> : FilterObjectStream<T, SentenceSample> where T : class {
        private readonly int chunkSize;
        private readonly IDetokenizer detokenizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractToSentenceSampleStream{T}"/> class.
        /// </summary>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <param name="samples">The samples.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <exception cref="System.ArgumentNullException">detokenizer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">chunkSize;@chunkSize must be zero or larger</exception>
        protected AbstractToSentenceSampleStream(IDetokenizer detokenizer, IObjectStream<T> samples, int chunkSize)
            : base(samples) {
            if (detokenizer == null)
                throw new ArgumentNullException(nameof(detokenizer));

            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException(nameof(chunkSize), chunkSize, @"chunkSize must be zero or larger");

            this.detokenizer = detokenizer;
            this.chunkSize = chunkSize > 0 ? chunkSize : int.MaxValue;
        }

        /// <summary>
        /// Converts to the sentence.
        /// </summary>
        /// <param name="sample">The sample.</param>
        /// <returns>The converted sentence.</returns>
        protected abstract string[] ToSentence(T sample);

        /// <summary>
        /// Returns the next <see cref="SentenceSample"/>. 
        /// Calling this method repeatedly until it returns, <c>null</c> will return each <see cref="SentenceSample"/>
        /// from the underlying source exactly once.
        /// </summary>
        /// <returns>The next <see cref="SentenceSample"/> or <c>null</c> to signal that the stream is exhausted.</returns>
        public override SentenceSample Read() {
            var sentences = new List<string[]>();


            T posSample;
            var chunks = 0;
            while ((posSample = Samples.Read()) != null && chunks < chunkSize) {
                sentences.Add(ToSentence(posSample));
                chunks++;
            }

            if (sentences.Count > 0)
                return new SentenceSample(detokenizer, sentences.ToArray());
            if (posSample != null)
                return Read(); // filter out empty line

            return null; // last sample was read
        }
    }
}