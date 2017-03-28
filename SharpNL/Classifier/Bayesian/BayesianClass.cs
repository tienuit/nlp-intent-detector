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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNL.Classifier.Bayesian {
    /// <summary>
    /// Represents a Bayesian class. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("BayesianClass - {Name} #{Count}")]
    public sealed class BayesianClass<F> : AbstractClass<F> {

        private readonly Dictionary<F, BayesianFeature<F>> knowledge;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="BayesianClass{T}" /> with its name.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/>
        /// </exception>
        public BayesianClass(string name) : base(name) {
            knowledge = new Dictionary<F, BayesianFeature<F>>();
            Comparer = Comparer<F>.Default;
        }
        #endregion

        #region + Properties .

        #region . Comparer .
        /// <summary>
        /// Gets or sets the feature comparer.
        /// </summary>
        /// <value>The feature comparer.</value>
        public IComparer Comparer { get; set; }
        #endregion

        #endregion

        #region . GetBayesianFeature .
        internal BayesianFeature<F> GetBayesianFeature(F feature, bool create) {
            if (knowledge.ContainsKey(feature)) {
                var bf = knowledge[feature];
                if (Comparer.Compare(bf.Feature, feature) == 0)
                    return bf;

            } else if (create) {
                var bf = new BayesianFeature<F>(feature);

                knowledge[feature] = bf;

                return bf;
            }

            return null;
        }
        #endregion

        #region . GetProbability .
        /// <summary>
        /// Gets the class probability for the given <paramref name="features"/>.
        /// </summary>
        /// <param name="features">The features to analyze.</param>
        /// <returns>The class probability.</returns>
        public double GetProbability(F[] features) {

            if (features == null || features.Length == 0)
                return BayesianSettings.NeutralProbability;

            var probs = GetFeatureProbabilities(features);

            var z = 0d;
            var xy = 0d;
            foreach (var p in probs) {

                z = z.Equals(0d)
                    ? 1 - p
                    : z * (1 - p);

                xy = xy.Equals(0d)
                    ? p
                    : xy * p;

            }

            return xy / (xy + z);
        }
        #endregion

        #region . GetProbabilities .
        /// <summary>
        /// Computes the probability of the given features.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <returns>The probability for each feature.</returns>
        public double[] GetFeatureProbabilities(F[] features) {
            if (features == null || features.Length == 0)
                return new double[0];

            var prob = new double[features.Length];
            for (var i = 0; i < features.Length; i++)
                prob[i] = knowledge.ContainsKey(features[i]) ? knowledge[features[i]].Probability : 0d;

            return prob;
        }
        #endregion

    }
}