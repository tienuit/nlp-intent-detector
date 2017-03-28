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

using System.Text;
using SharpNL.Extensions;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Lemmatizer utilities.
    /// </summary>
    public static class LemmatizerUtils {
        /// <summary>
        /// Read predicted SES by the lemmatizer model and apply the permutations to obtain the lemma from the wordForm.
        /// </summary>
        /// <param name="wordForm">The word form</param>
        /// <param name="permutations">The permutations predicted by the lemmatizer model.</param>
        /// <returns>The lemma</returns>
        public static string DecodeShortestEditScript(string wordForm, string permutations) {
            var lemma = new StringBuilder(wordForm.Reverse());

            var permIndex = 0;
            while (true) {
                if (permutations.Length <= permIndex)
                    break;

                // Read first letter of permutation string
                var nextOperation = permutations[permIndex];
                // Go to the next permutation letter
                permIndex++;

                var charAtPerm = permutations[permIndex].ToString();
                var charIndex = int.Parse(charAtPerm);

                switch (nextOperation) {
                    case 'R':
                        // go to the next character in the permutation buffer
                        // which is the replacement character
                        permIndex++;
                        var replace = permutations[permIndex];

                        //go to the next char in the permutation buffer
                        // which is the candidate character
                        permIndex++;
                        var with = permutations[permIndex];

                        if (lemma.Length <= charIndex)
                            return wordForm;

                        if (lemma[charIndex] == replace)
                            lemma[charIndex] = with;

                        permIndex++;
                        break;
                    case 'I':
                        permIndex++;
                        //character to be inserted
                        var ins = permutations[permIndex];

                        if (lemma.Length < charIndex)
                            return wordForm;

                        lemma.Insert(charIndex, ins);

                        permIndex++;
                        break;
                    case 'D':
                        if (lemma.Length <= charIndex)
                            return wordForm;

                        lemma.Remove(charIndex, 1);
                        permIndex++;

                        // go to next permutation
                        permIndex++;
                        break;
                }
            }
            return lemma.ToString().Reverse();
        }


        private static int Minimum(int a, int b, int c) {
            var minValue = a;
            if (b < minValue) {
                minValue = b;
            }
            if (c < minValue) {
                minValue = c;
            }
            return minValue;
        }

        /// <summary>
        /// Computes the Levenshtein distance of two strings in a matrix.
        /// </summary>
        /// <param name="wordForm">The form</param>
        /// <param name="lemma">The lemma</param>
        /// <returns>The distance</returns>
        /// <remarks>
        /// Based on pseudo-code provided here:
        /// https://en.wikipedia.org/wiki/Levenshtein_distance#Computing_Levenshtein_distance
        /// which in turn is based on the paper Wagner, Robert A.; Fischer, Michael J. (1974),
        /// "The String-to-String Correction Problem", Journal of the ACM 21 (1): 168-173
        /// </remarks>
        public static int[,] LevenshteinDistance(string wordForm, string lemma) {
            var wordLength = wordForm.Length;
            var lemmaLength = lemma.Length;
            var distance = new int[wordLength + 1, lemmaLength + 1];

            if (wordLength == 0) {
                return distance;
            }
            if (lemmaLength == 0) {
                return distance;
            }
            //fill in the rows of column 0
            for (var i = 0; i <= wordLength; i++) {
                distance[i, 0] = i;
            }
            //fill in the columns of row 0
            for (var j = 0; j <= lemmaLength; j++) {
                distance[0, j] = j;
            }
            //fill in the rest of the matrix calculating the minimum distance
            for (var i = 1; i <= wordLength; i++) {
                int c = wordForm[i - 1];
                for (var j = 1; j <= lemmaLength; j++) {
                    var cost = c == lemma[j - 1] ? 0 : 1;

                    //obtain minimum distance from calculating deletion, insertion, substitution
                    distance[i, j] = Minimum(distance[i - 1, j] + 1, distance[i, j - 1] + 1, distance[i - 1, j - 1] + cost);
                }
            }
            return distance;
        }

        /// <summary>
        /// Computes the Shortest Edit Script (SES) to convert a word into its lemma.
        /// </summary>
        /// <param name="wordForm">The token</param>
        /// <param name="lemma">The target lemma.</param>
        /// <param name="distance">The levenshtein distance.</param>
        /// <param name="permutations">The permutations.</param>
        /// <remarks>
        /// This is based on Chrupala's PhD thesis (2008).
        /// </remarks>
        public static void ComputeShortestEditScript(string wordForm, string lemma, int[,] distance, StringBuilder permutations) {
            var n = distance.GetLength(0);
            var m = distance.GetLength(1);

            var wordFormLength = n - 1;
            var lemmaLength = m - 1;
            while (true) {
                if (distance[wordFormLength, lemmaLength] == 0) {
                    break;
                }

                if (lemmaLength > 0 && wordFormLength > 0 && (distance[wordFormLength - 1, lemmaLength - 1] < distance[wordFormLength, lemmaLength])) {
                    permutations.Append('R').Append((wordFormLength - 1).ToString()).Append(wordForm[wordFormLength - 1]).Append(lemma[lemmaLength - 1]);
                    lemmaLength--;
                    wordFormLength--;
                    continue;
                }
                if (lemmaLength > 0 && (distance[wordFormLength, lemmaLength - 1] < distance[wordFormLength, lemmaLength])) {
                    permutations.Append('I').Append(wordFormLength.ToString()).Append(lemma[lemmaLength - 1]);
                    lemmaLength--;
                    continue;
                }
                if (wordFormLength > 0 && (distance[wordFormLength - 1, lemmaLength] < distance[wordFormLength, lemmaLength])) {
                    permutations.Append('D').Append((wordFormLength - 1).ToString()).Append(wordForm[wordFormLength - 1]);
                    wordFormLength--;
                    continue;
                }
                if (wordFormLength > 0 && lemmaLength > 0 && (distance[wordFormLength - 1, lemmaLength - 1] == distance[wordFormLength, lemmaLength])) {
                    wordFormLength--;
                    lemmaLength--;
                    continue;
                }
                if (wordFormLength > 0 && (distance[wordFormLength - 1, lemmaLength] == distance[wordFormLength, lemmaLength])) {
                    wordFormLength--;
                    continue;
                }
                if (lemmaLength > 0 && (distance[wordFormLength, lemmaLength - 1] == distance[wordFormLength, lemmaLength])) {
                    lemmaLength--;
                }
            }
        }

        /// <summary>
        /// Get the SES required to go from a word to a lemma.
        /// </summary>
        /// <param name="wordForm">The work form</param>
        /// <param name="lemma">The lemma.</param>
        /// <returns>the shortest edit script</returns>
        public static string GetShortestEditScript(string wordForm, string lemma) {
            var reverseWord = wordForm.ToLowerInvariant().Reverse();
            var reversedLemma = lemma.ToLowerInvariant().Reverse();

            if (reverseWord.Equals(reversedLemma))
                return "O";

            var permutations = new StringBuilder();
            var levenDistance = LevenshteinDistance(reverseWord, reversedLemma);

            ComputeShortestEditScript(reverseWord, reversedLemma, levenDistance, permutations);

            return permutations.ToString();
        }
    }
}