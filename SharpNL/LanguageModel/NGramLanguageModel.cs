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
using System.IO;
using SharpNL.NGram;
using SharpNL.Utility;

namespace SharpNL.LanguageModel {
    /// <summary>
    ///     A <see cref="ILanguageModel" /> based on a <see cref="NGramModel" /> using smoothing probability estimation to get
    ///     the probabilities of the ngrams.
    /// </summary>
    public class NGramLanguageModel : NGramModel, ILanguageModel {
        public const int DefaultN = 3;
        public const double DefaultK = 1d;
        private readonly double k;

        private readonly int n;

        public NGramLanguageModel() : this(DefaultN, DefaultK) {
        }

        public NGramLanguageModel(int n) : this(n, DefaultK) {
        }

        public NGramLanguageModel(double k) : this(DefaultN, k) {
        }

        public NGramLanguageModel(int n, double k) {
            this.n = n;
            this.k = k;
        }


        public NGramLanguageModel(Stream inputStream, int n) : this(inputStream, n, DefaultK) {
        }

        public NGramLanguageModel(Stream inputStream, double k) : this(inputStream, DefaultN, k) {
        }


        public NGramLanguageModel(Stream inputStream, int n, double k) : base(inputStream) {
            this.n = n;
            this.k = k;
        }


        public double CalculateProbability(StringList sample) {
            if (Count <= 0)
                return 0d;

            var probability = 0d;
            foreach (var ngram in NGramUtils.GetNGrams(sample, n)) {
                var nMinusOneToken = NGramUtils.GetNMinusOneTokenFirst(ngram);
                if (Count > 1000000) {
                    // use stupid backoff
                    probability += Math.Log(GetStupidBackoffProbability(ngram, nMinusOneToken));
                } else {
                    // use laplace smoothing
                    probability += Math.Log(GetLaplaceSmoothingProbability(ngram, nMinusOneToken));
                }
            }
            if (double.IsNaN(probability)) {
                probability = 0d;
            } else if (Math.Abs(probability) > 0.000001) {
                probability = Math.Exp(probability);
            }
            return probability;
        }

        public StringList PredictNextTokens(StringList tokens) {
            var maxProb = double.NegativeInfinity;
            StringList token = null;

            foreach (var ngram in this) {
                var sequence = new string[ngram.Count + tokens.Count];
                for (var i = 0; i < tokens.Count; i++) {
                    sequence[i] = tokens[i];
                }
                for (var i = 0; i < ngram.Count; i++) {
                    sequence[i + tokens.Count] = ngram[i];
                }
                var sample = new StringList(sequence);
                var v = CalculateProbability(sample);
                if (v > maxProb) {
                    maxProb = v;
                    token = ngram;
                }
            }

            return token;
        }

        private double GetLaplaceSmoothingProbability(StringList ngram, StringList nMinusOneToken) {
            return (GetCount(ngram) + k)/(GetCount(nMinusOneToken) + k*Count);
        }

        private double GetStupidBackoffProbability(StringList ngram, StringList nMinusOneToken) {
            var count = GetCount(ngram);
            if (nMinusOneToken == null || nMinusOneToken.Count == 0) {
                return (double)count/Count;
            }
            if (count > 0) {
                return count/(double) GetCount(nMinusOneToken); // maximum likelihood probability
            }
            var nextNgram = NGramUtils.GetNMinusOneTokenLast(ngram);
            return 0.4d*GetStupidBackoffProbability(nextNgram, NGramUtils.GetNMinusOneTokenFirst(nextNgram));
        }
    }
}