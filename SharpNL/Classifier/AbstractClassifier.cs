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
using System.Linq;

namespace SharpNL.Classifier {
    /// <summary>
    /// Represents a abstract classifier.
    /// </summary>
    /// <typeparam name="C">The class type.</typeparam>
    /// <typeparam name="R">The result type.</typeparam>
    /// <typeparam name="F">The feature type.</typeparam>
    public abstract class AbstractClassifier<C, R, F> : IClassClassifier<C, R, F> 
        where R : IClassClassifierResult<C, F>
        where C : IClass<F> {

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractClassifier{C, R, F}"/> class.
        /// </summary>
        protected AbstractClassifier() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractClassifier{C, R, F}"/> using a specific <see cref="IEqualityComparer{F}"/> in the <see cref="IgnoredFeatures"/>.
        /// </summary>
        /// <param name="comparer">The equality comparer.</param>
        protected AbstractClassifier(IEqualityComparer<F> comparer) {
            ignoredFeatures = new HashSet<F>(comparer);
        }
        #endregion

        #region + Properties .

        #region . Classes .
        private HashSet<C> classes;
        /// <summary>
        /// Gets the classes in this classifier.
        /// </summary>
        /// <value>The classes in this classifier.</value>
        public HashSet<C> Classes => classes ?? (classes = new HashSet<C>());

        #endregion

        #region . IgnoredFeatures .
        private HashSet<F> ignoredFeatures;
        /// <summary>
        /// Gets the ignored features by this classifier.
        /// </summary>
        /// <value>The ignored features by this classifier.</value>
        public HashSet<F> IgnoredFeatures => ignoredFeatures ?? (ignoredFeatures = new HashSet<F>());

        #endregion

        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified features classes.
        /// </summary>
        /// <param name="features">The features to be classified.</param>
        /// <returns>The classification values for the features, the higher, the better is the match.</returns>
        protected abstract SortedSet<R> Evaluate(F[] features);
        #endregion

        #region . Classify .
        /// <summary>
        /// Classifies the specified <paramref name="features"/>.
        /// </summary>
        /// <param name="features">The features to be classified.</param>
        /// <returns>The classification values for the features, the higher, the better is the match.</returns>
        public SortedSet<R> Classify(params F[] features) {
            if (classes == null || classes.Count == 0)
                return new SortedSet<R>();

            return Evaluate(Prepare(features));
        }
        #endregion

        #region . GetBestResult .
        /// <summary>
        /// Gets the name of the class corresponding to the highest likelihood.
        /// </summary>
        /// <param name="features">The features to be evaluated.</param>
        /// <returns>The string name of the best class or a <c>null</c> value.</returns>
        public R GetBestResult(params F[] features) {
            var set = Classify(features);

            if (set == null || set.Count == 0)
                return default(R);

            var best = default(R);

            foreach (var r in set) {
                if (ReferenceEquals(default(R), best) || best.Probability < r.Probability)
                    best = r;
            }

            return best;

        }
        #endregion

        #region + GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through the classes.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<C> GetEnumerator() {
            if (classes == null)
                yield break;

            foreach (var c in classes)
                yield return c;
        }
        /// <summary>
        /// Returns an enumerator that iterates through the classes.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region . IsIgnored .

        /// <summary>
        /// Determines whether the specified feature is ignored.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if the specified feature is ignored; otherwise, <c>false</c>.</returns>
        public bool IsIgnored(F feature) {
            return ignoredFeatures != null && ignoredFeatures.Contains(feature);
        }

        #endregion

        #region . Prepare .
        /// <summary>
        /// Prepares the specified features to the classification.
        /// </summary>
        /// <param name="features">The features to be classified.</param>
        /// <returns>The features without the ignored features.</returns>
        protected virtual F[] Prepare(F[] features) {
            return ignoredFeatures != null
                ? features.Where(feature => !ignoredFeatures.Any(f => f.Equals(feature))).ToArray()
                : features;
        }
        #endregion


    }
}