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
//  The bayesian classifier is inspired on Nick Lothian's - Classifier4J implementation 
//

namespace SharpNL.Classifier.Bayesian {
    /// <summary>
    /// Represents the bayesian theorem settings.
    /// </summary>
    public static class BayesianSettings {

        /// <summary>
        /// The default class name.
        /// </summary>
        public static string DefaultClass = "Default";

        /// <summary>
        /// The neutral probability of a bayesian feature.
        /// </summary>
        public static double NeutralProbability = .5d;

        /// <summary>
        /// The minimum likelihood that a feature matches.
        /// </summary>
        public static double LowerBound = .01d;

        /// <summary>
        /// The maximum likelihood that a feature matches.
        /// </summary>
        public static double UpperBound = .99d;

        /// <summary>
        /// The cutoff value used by the classifier. Any match probability greater than or equal to this value
        /// will be classified as a match.
        /// </summary>
        public static double DefaultCutoff = .9d;

        #region . Normalize .
        /// <summary>
        /// Normalizes the specified probability.
        /// </summary>
        /// <param name="p">The probability.</param>
        /// <returns>The normalyzed probability.</returns>
        internal static double Normalize(double p) {
            return UpperBound < p ? UpperBound : LowerBound > p ? LowerBound : p;
        }
        #endregion

    }
}