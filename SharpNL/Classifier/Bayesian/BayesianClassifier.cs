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

using System;
using System.Collections.Generic;

namespace SharpNL.Classifier.Bayesian {
    /// <summary>
    /// A implementation a classifier based on Bayes' theorem.
    /// </summary>
    /// <seealso href="http://en.wikipedia.org/wiki/Bayes%27_theorem"/>
    public class BayesianClassifier<F> : AbstractClassifier<BayesianClass<F>, BayesianResult<F>,  F> {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="BayesianClassifier{F}"/> class using the default cutoff.
        /// </summary>
        public BayesianClassifier() {
            cutoff = BayesianSettings.DefaultCutoff;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BayesianClassifier{F}"/> class with the specified cutoff value.
        /// </summary>
        /// <param name="cutoff">The cutoff.</param>
        public BayesianClassifier(double cutoff) {
            Cutoff = cutoff;
        }
        #endregion

        #region + Properties .

        #region . Cutoff .

        private double cutoff;

        /// <summary>
        /// Gets or sets the classification cutoff.
        /// </summary>
        /// <value>The classification cutoff. The default value is <see cref="BayesianSettings.DefaultCutoff"/>.</value>
        public double Cutoff {
            get { return cutoff; }
            set { cutoff = BayesianSettings.Normalize(value); }
        }
        #endregion

        #endregion

        #region + TeachMatch .

        /// <summary>
        /// Teaches the given features as matching in the default class.
        /// </summary>
        /// <param name="features">The matching features.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="features"/>
        /// </exception>
        public void TeachMatch(F[] features) {
            TeachMatch(BayesianSettings.DefaultClass, features);
        }

        /// <summary>
        /// Teaches the given features as matching in the specified class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <param name="features">The matching features.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="className"/>
        /// or
        /// <paramref name="features"/>
        /// </exception>
        public void TeachMatch(string className, F[] features) {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentNullException(nameof(className));

            if (features == null)
                throw new ArgumentNullException(nameof(features));

            var bc = GetClass(className, true);
            foreach (var feature in features) {
                if (IsIgnored(feature)) continue;

                var bf = bc.GetBayesianFeature(feature, true);
                bf.Matching++;
            }
        }
        #endregion

        #region + TeachNonMatch .
        /// <summary>
        /// Teaches the given features as non matching in the default class.
        /// </summary>
        /// <param name="features">The non matching features.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="features"/>
        /// </exception>
        public void TeachNonMatch(F[] features) {
            TeachNonMatch(BayesianSettings.DefaultClass, features);
        }

        /// <summary>
        /// Teaches the given features as non matching in the specified class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="features">The non matching features.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="className"/>
        /// or
        /// <paramref name="features"/>
        /// </exception>
        public void TeachNonMatch(string className, F[] features) {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentNullException(nameof(className));

            if (features == null)
                throw new ArgumentNullException(nameof(features));

            var bc = GetClass(className, true);
            foreach (var feature in features) {
                if (IsIgnored(feature)) continue;

                var bf = bc.GetBayesianFeature(feature, true);
                bf.NonMatching++;
            }
        }
        #endregion

        #region + SetFeatureProbability .

        /// <summary>
        /// Sets the feature probability in the default class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="probability">The feature probability.</param>
        public void SetFeatureProbability(F feature, double probability) {
            SetFeatureProbability(BayesianSettings.DefaultClass, feature, probability);
        }

        /// <summary>
        /// Sets the feature probability in a specific class.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="probability">The feature probability.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="className"/>
        /// </exception>
        public void SetFeatureProbability(string className, F feature, double probability) {

            if (string.IsNullOrEmpty(className))
                throw new ArgumentNullException(nameof(className));

            var c = GetClass(className, true);

            var f = c.GetBayesianFeature(feature, true);

            f.Probability = probability;
        }
        #endregion

        #region . GetClass .
        /// <summary>
        /// Gets the bayesian class by the class name.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <param name="create">if set to <c>true</c> the class will be created if it cannot be found.</param>
        /// <returns>The <see cref="BayesianClass{F}"/> or a <c>null</c> value.</returns>
        protected BayesianClass<F> GetClass(string name, bool create) {

            foreach (var cls in Classes) {
                if (cls.Name == name)
                    return cls;
            }

            if (!create)
                return null;

            var bc = new BayesianClass<F>(name);
            Classes.Add(bc);
            return bc;
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified features classes.
        /// </summary>
        /// <param name="features">The features to be classified.</param>
        /// <returns>The classification values for the features, the higher, the better is the match.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override SortedSet<BayesianResult<F>> Evaluate(F[] features) {
            if (Classes.Count == 0)
                return new SortedSet<BayesianResult<F>>();

            var set = new SortedSet<BayesianResult<F>>();
            foreach (var c in Classes) {
                var p = c.GetProbability(features);

                if (p > BayesianSettings.DefaultCutoff)
                    set.Add(new BayesianResult<F>(c, p));

            }
                

            return set;
        }
        #endregion

    }
}