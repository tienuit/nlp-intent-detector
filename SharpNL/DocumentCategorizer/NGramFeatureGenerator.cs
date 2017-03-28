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
using SharpNL.Utility;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Represents a nGram feature generator.
    /// </summary>   
    [TypeClass("opennlp.tools.doccat.NGramFeatureGenerator")]
    public class NGramFeatureGenerator : IFeatureGenerator {
        /// <summary>
        /// The default minimum words in a ngram feature.
        /// </summary>
        public static int DefaultMinGram { get; } 

        /// <summary>
        /// The default maximum words in a ngram feature.
        /// </summary>
        public static int DefaultMaxGram { get; }


        /// <summary>
        /// Initializes static members of the <see cref="NGramFeatureGenerator"/> class.
        /// </summary>
        static NGramFeatureGenerator() {
            DefaultMinGram = 2;
            DefaultMaxGram = 2;
        }

        private readonly int minGram;
        private readonly int maxGram;

        /// <summary>
        /// Initializes a new instance of the <see cref="NGramFeatureGenerator"/> using the default parameters.
        /// </summary>
        public NGramFeatureGenerator() {
            minGram = DefaultMinGram;
            maxGram = DefaultMaxGram;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NGramFeatureGenerator"/> class.
        /// </summary>
        /// <param name="minGram">The minimum words in a ngram feature.</param>
        /// <param name="maxGram">The maximum words in a ngram feature.</param>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="minGram"/> value must be greater then zero.
        /// or
        /// The <paramref name="maxGram"/> value must be greater then zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">minGram</exception>
        public NGramFeatureGenerator(int minGram, int maxGram) {
            if (minGram < 0)
                throw new ArgumentException("The value must be greater then zero.", nameof(minGram));

            if (maxGram < 0)
                throw new ArgumentException("The value must be greater then zero.", nameof(maxGram));

            if (minGram > maxGram)
                throw new ArgumentOutOfRangeException(nameof(minGram));

            this.minGram = minGram;
            this.maxGram = maxGram;

        }

        /// <summary>
        /// Extracts the features from the given text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>The list of features.</returns>
        public List<string> ExtractFeatures(string[] text, Dictionary<string, object> extraInformation) {
            var features = new List<string>();

            for (var i = 0; i <= text.Length - minGram; i++) {
                var feature = "ng=";
                for (var y = 0; y < maxGram && i + y < text.Length; y++) {
                    feature = feature + ":" + text[i + y];
                    var gramCount = y + 1;
                    if (maxGram >= gramCount && gramCount >= minGram)
                        features.Add(feature);
                    
                }
            }
            return features;
        }
    }
}