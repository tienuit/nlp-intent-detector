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
using System.Diagnostics;

namespace SharpNL.Classifier.Bayesian {
    /// <summary>
    /// Represents a bayesian class feature. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("BayesianFeature - {Feature} [ {Matching} / {NonMatching} ]")]
    internal sealed class BayesianFeature<F> : IEquatable<BayesianFeature<F>> {


        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="BayesianFeature{T}"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <exception cref="System.ArgumentNullException">feature</exception>
        public BayesianFeature(F feature) {
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BayesianFeature{T}"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="matching">The matching count.</param>
        /// <param name="nonMatching">The non matching count.</param>
        /// <exception cref="System.ArgumentNullException">feature</exception>
        public BayesianFeature(string feature, double matching, double nonMatching) {
            if (string.IsNullOrEmpty(feature))
                throw new ArgumentNullException(nameof(feature));

            this.matching = matching;
            this.nonMatching = nonMatching;
            CalculateProbability();
        }
        #endregion

        #region + Properties .

        #region . Feature .
        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>The feature.</value>
        public F Feature { get; private set; }
        #endregion

        #region . Probability .

        private double probability = BayesianSettings.NeutralProbability;
        /// <summary>
        /// Gets or sets the feature probability.
        /// </summary>
        /// <value>The feature probability.</value>
        public double Probability {
            get { return probability; }
            set {
                probability = BayesianSettings.Normalize(value);

                matching = 0;
                nonMatching = 0;
            }
        }
        #endregion

        #region . Matching .

        private double matching;
        /// <summary>
        /// Gets the number of times that this feature matches.
        /// </summary>
        /// <value>The number of times that this feature matches.</value>
        public double Matching {
            get { return matching; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                matching = value;
                CalculateProbability();
            }

        }
        #endregion

        #region . NonMatching .
        private double nonMatching;

        /// <summary>
        /// Gets the number of times that this feature does not match.
        /// </summary>
        /// <value>The number of times that this feature foes not match.</value>
        public double NonMatching {
            get { return nonMatching; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                nonMatching = value;
                CalculateProbability();
            }
        }
        #endregion

        #endregion

        #region . ComputeProbability .
        private void CalculateProbability() {
            if (matching.Equals(0d))
                probability = nonMatching.Equals(0d) 
                    ? BayesianSettings.NeutralProbability 
                    : BayesianSettings.LowerBound;
            else
                probability = BayesianSettings.Normalize(matching / (matching + nonMatching));
        }
        #endregion

        #region . Normalize .

        #endregion

        #region + Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(BayesianFeature<F> other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Feature.Equals(other.Feature);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BayesianFeature<F> && Equals((BayesianFeature<F>)obj);
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return (matching.GetHashCode() * 397) ^ nonMatching.GetHashCode();
            }
        }
        #endregion

    }
}