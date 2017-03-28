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
using SharpNL.Utility;

namespace SharpNL.NGram {
    /// <summary>
    ///     Utility class for ngrams.
    ///     Some methods apply specifically to certain 'n' values, for e.g. tri/bi/uni-grams.
    /// </summary>
    public static class NGramUtils {
        /// <summary>
        ///     Calculate the probability of a ngram in a vocabulary using Laplace smoothing algorithm
        /// </summary>
        /// <param name="ngram">The ngram to get the probability for.</param>
        /// <param name="set">The vocabulary.</param>
        /// <param name="size">The size of the vocabulary.</param>
        /// <param name="k">The smoothing factor.</param>
        /// <returns>The Laplace smoothing probability.</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Additive_smoothing">Additive Smoothing</seealso>
        public static double CalculateLaplaceSmoothingProbability(StringList ngram, IList<StringList> set,
            int size, double k) {
            return (Count(ngram, set) + k)/(Count(GetNMinusOneTokenFirst(ngram), set) + k*1);
        }

        /// <summary>
        ///     Calculate the probability of a unigram in a vocabulary using maximum likelihood estimation
        /// </summary>
        /// <param name="word">The only word in the unigram</param>
        /// <param name="set">The vocabulary</param>
        /// <returns>The maximum likelihood probability</returns>
        public static double CalculateUnigramMLProbability(string word, IList<StringList> set) {
            var vocSize = set.Aggregate(0d, (current, s) => current + s.Count);
            return Count(new StringList(word), set)/vocSize;
        }

        /// <summary>
        ///     Calculate the probability of a bigram in a vocabulary using maximum likelihood estimation.
        /// </summary>
        /// <param name="x0">The first word in the bigram</param>
        /// <param name="x1">The second word in the bigram</param>
        /// <param name="set">the vocabulary</param>
        /// <returns>The maximum likelihood probability</returns>
        public static double CalculateBigramMLProbability(string x0, string x1, IList<StringList> set) {
            return CalculateNgramMLProbability(new StringList(x0, x1), set);
        }

        /// <summary>
        ///     Calculate the probability of a trigram in a vocabulary using maximum likelihood estimation.
        /// </summary>
        /// <param name="x0">The first word in the trigram.</param>
        /// <param name="x1">The second word in the trigram.</param>
        /// <param name="x2">The third word in the trigram.</param>
        /// <param name="set">The vocabulary.</param>
        /// <returns>The maximum likelihood probability.</returns>
        public static double CalculateTrigramMLProbability(string x0, string x1, string x2, IList<StringList> set) {
            return CalculateNgramMLProbability(new StringList(x0, x1, x2), set);
        }

        /// <summary>
        ///     Calculate the probability of a ngram in a vocabulary using maximum likelihood estimation.
        /// </summary>
        /// <param name="ngram">A ngram</param>
        /// <param name="set">The vocabulary</param>
        /// <returns>The maximum likelihood probability.</returns>
        public static double CalculateNgramMLProbability(StringList ngram, IList<StringList> set) {
            var ngramMinusOne = GetNMinusOneTokenFirst(ngram);
            return Count(ngram, set)/Count(ngramMinusOne, set);
        }

        /// <summary>
        ///     Calculate the probability of a bigram in a vocabulary using prior Laplace smoothing algorithm.
        /// </summary>
        /// <param name="x0">The first word in the bigram.</param>
        /// <param name="x1">The second word in the bigram.</param>
        /// <param name="set">The vocabulary.</param>
        /// <param name="k">The smoothing factor.</param>
        /// <returns>The prior Laplace smoothiing probability.</returns>
        public static double CalculateBigramPriorSmoothingProbability(string x0, string x1, IList<StringList> set,
            double k) {
            return (Count(new StringList(x0, x1), set) + k*CalculateUnigramMLProbability(x1, set))/
                   (Count(new StringList(x0), set) + k*set.Count);
        }

        /// <summary>
        ///     Calculate the probability of a trigram in a vocabulary using a linear interpolation algorithm
        /// </summary>
        /// <param name="x0">The first word in the trigram</param>
        /// <param name="x1">The second word in the trigram</param>
        /// <param name="x2">The third word in the trigram</param>
        /// <param name="set">The vocabulary</param>
        /// <param name="lambda1">Trigram interpolation factor</param>
        /// <param name="lambda2">Bigram interpolation factor</param>
        /// <param name="lambda3">Unigram interpolation factor</param>
        /// <returns>The linear interpolation probability</returns>
        public static double CalculateTrigramLinearInterpolationProbability(string x0, string x1, string x2,
            IList<StringList> set,
            double lambda1, double lambda2, double lambda3) {
            if (Math.Abs(lambda1 + lambda2 + lambda3 - 1d) > 0.00001)
                throw new ArgumentException("Lambdas sum should be equals to 1");

            if (lambda1 + lambda2 + lambda3 <= 0d)
                throw new ArgumentException("Lambdas should all be greater than 0.");


            return lambda1*CalculateTrigramMLProbability(x0, x1, x2, set) +
                   lambda2*CalculateBigramMLProbability(x1, x2, set) +
                   lambda3*CalculateUnigramMLProbability(x2, set);
        }

        /// <summary>
        ///     Calculate the probability of a ngram in a vocabulary using the missing probability mass algorithm
        /// </summary>
        /// <param name="ngram">The Ngram</param>
        /// <param name="discount">discount factor</param>
        /// <param name="set">the vocabulary</param>
        /// <returns>the probability</returns>
        public static double CalculateMissingNgramProbabilityMass(StringList ngram, double discount,
            IList<StringList> set) {
            var missingMass = 0d;
            var countWord = Count(ngram, set);
            foreach (var word in FlatSet(set)) {
                missingMass += (Count(GetNPlusOneNgram(ngram, word), set) - discount)/countWord;
            }
            return 1 - missingMass;
        }

        /// <summary>
        ///     get the (n-1)th ngram of a given ngram, that is the same ngram except the last word in the ngram
        /// </summary>
        /// <param name="ngram">A Ngram</param>
        /// <returns>A Ngram</returns>
        public static StringList GetNMinusOneTokenFirst(StringList ngram) {
            var tokens = new string[ngram.Count - 1];
            for (var i = 0; i < ngram.Count - 1; i++) {
                tokens[i] = ngram[i];
            }
            return tokens.Length > 0 ? new StringList(tokens) : null;
        }

        /// <summary>
        ///     Get the (n-1)th ngram of a given ngram, that is the same ngram except the first word in the ngram
        /// </summary>
        /// <param name="ngram">A Ngram</param>
        /// <returns>A Ngram</returns>
        public static StringList GetNMinusOneTokenLast(StringList ngram) {
            var tokens = new string[ngram.Count - 1];
            for (var i = 1; i < ngram.Count; i++) {
                tokens[i - 1] = ngram[i];
            }
            return tokens.Length > 0 ? new StringList(tokens) : null;
        }

        private static StringList GetNPlusOneNgram(StringList ngram, string word) {
            var tokens = new string[ngram.Count + 1];
            for (var i = 0; i < ngram.Count; i++) {
                tokens[i] = ngram[i];
            }
            tokens[tokens.Length - 1] = word;
            return new StringList(tokens);
        }


        private static double Count(StringList ngram, IList<StringList> sentences) {
            var count = 0d;
            foreach (var sentence in sentences) {
                var idx0 = IndexOf(sentence, ngram[0]);
                if (idx0 >= 0 && sentence.Count >= idx0 + ngram.Count) {
                    var match = true;
                    for (var i = 1; i < ngram.Count; i++) {
                        var sentenceToken = sentence[idx0 + i];
                        var ngramToken = ngram[i];
                        match &= sentenceToken.Equals(ngramToken);
                    }
                    if (match)
                        count++;
                }
            }
            return count;
        }

        private static int IndexOf(StringList sentence, string token) {
            for (var i = 0; i < sentence.Count; i++) {
                if (token.Equals(sentence[i])) {
                    return i;
                }
            }
            return -1;
        }

        private static IList<string> FlatSet(IList<StringList> set) {
            return set.SelectMany(sentence => sentence).ToList();
        }

        /// <summary>
        ///     Get the ngrams of dimension n of a certain input sequence of tokens
        /// </summary>
        /// <param name="sequence">A sequence of tokens</param>
        /// <param name="size">the size of the resulting ngrmams</param>
        /// <returns>All the possible ngrams of the given size derivable from the input sequence</returns>
        public static IList<StringList> GetNGrams(StringList sequence, int size) {
            var ngrams = new List<StringList>();
            if (size == -1 || size >= sequence.Count) {
                ngrams.Add(sequence);
            } else {
                var ngram = new string[size];
                for (var i = 0; i < sequence.Count - size + 1; i++) {
                    ngram[0] = sequence[i];
                    for (var j = 1; j < size; j++) {
                        ngram[j] = sequence[i + j];
                    }
                    ngrams.Add(new StringList(ngram));
                }
            }

            return ngrams;
        }
    }
}