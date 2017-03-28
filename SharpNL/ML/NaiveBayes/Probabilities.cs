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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Represents the probability distribution over labels returned by a classifier.
    /// </summary>
    /// <typeparam name="T">The label type.</typeparam>
    public abstract class Probabilities<T> {

        /// <summary>
        /// The probabilities mapping
        /// </summary>
        protected readonly IDictionary<T, double> Map;

        protected Probabilities() : this(new Dictionary<T, double>()) {
            
        }
        protected Probabilities(IDictionary<T, double> map) {
            Map = map;
        }


        #region + Properties .

        #region . Confidence .
        /// <summary>
        /// Gets or sets the best confidence with which this set of probabilities has been calculated.
        /// </summary>
        /// <value>The best confidence of the probabilities.</value>
        /// <remarks>
        /// This is a function of the amount of data that supports the assertion.
        /// It is also a measure of the accuracy of the estimator of the probability.
        /// </remarks>
        public double Confidence { get; set; }
        #endregion

        #region . Max .
        /// <summary>
        /// Gets the most likely label.
        /// </summary>
        /// <value>The label that has the highest associated probability.</value>
        public virtual T Max {
            get {
                var max = 0d;
                var label = default(T);

                foreach (var pair in Map) {
                    if (pair.Value < max) 
                        continue;

                    label = pair.Key;
                    max = pair.Value;
                }

                return label;
            }
        }
        #endregion

        #region . MaxValue .
        /// <summary>
        /// Gets the probability of the most likely label
        /// </summary>
        /// <value>The highest probability.</value>
        public virtual double MaxValue => Map.Values.Max();

        #endregion

        #region . Normalized .

        private IDictionary<T, double> normalized;

        /// <summary>
        /// Gets the normalized probability map.
        /// </summary>
        /// <value>The normalized map.</value>
        protected IDictionary<T, double> Normalized {
            get { return normalized ?? (normalized = Normalize()); }
            set { normalized = value; }
        }
        #endregion


        #endregion

        #region . AddIn .

        /// <summary>
        /// Compounds the existing probability mass on the label with the new probability passed in to the method.
        /// </summary>
        /// <param name="label">The label whose probability mass is being updated.</param>
        /// <param name="probability">The probability weight to add.</param>
        /// <param name="count">The amplifying factor for the probability compounding.</param>
        public virtual void AddIn(T label, double probability, int count) {
            normalized = null;

            var p = Map.ContainsKey(label) ? Map[label] : 1d;

            probability = Math.Pow(probability, count);

            Map[label] = p*probability;
        }

        #endregion

        #region . CreateMapDataStructure .
        /// <summary>
        /// Creates the map data structure.
        /// </summary>
        /// <returns>A implemented dictionary.</returns>
        protected virtual IDictionary<T, double> CreateMapDataStructure() {
            return new Dictionary<T, double>();
        }
        #endregion

        #region . DiscardCountsBelow .
        public virtual void DiscardCountsBelow(double p) {
            var remove = Map.Where(pair => pair.Value < p);
            foreach (var pair in remove)
                Map.Remove(pair.Key);
        }
        #endregion

        #region . Get .
        /// <summary>
        /// Gets the probability associated with a label.
        /// </summary>
        /// <param name="label">The label whose probability needs to be returned.</param>
        /// <returns>The probability associated with the label.</returns>
        public virtual double Get(T label) {
            return !Map.ContainsKey(label) ? 0d : Normalize()[label];
        }
        #endregion

        #region . GetLog .
        /// <summary>
        /// Gets the log probability associated with a label.
        /// </summary>
        /// <param name="label">The label whose log probability needs to be returned.</param>
        /// <returns>The log probability associated with the label.</returns>
        public virtual double GetLog(T label) {
            return Math.Log(Get(label));
        }

        #endregion

        #region + Set .
        /// <summary>
        /// Assigns a probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="label">The label to which the probability is being assigned.</param>
        /// <param name="probability">The probability to assign.</param>
        public virtual void Set(T label, double probability) {
            normalized = null;
            Map[label] = probability;
        }

        /// <summary>
        /// Assigns a probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="label">The label to which the probability is being assigned.</param>
        /// <param name="probability">The probability to assign.</param>
        public virtual void Set(T label, Probability<T> probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));

            normalized = null;
            Map[label] = probability.Value;
        }
        #endregion

        #region . SetIfLarger .
        /// <summary>
        /// Assigns a probability to a label, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="label">The label to which the probability is being assigned.</param>
        /// <param name="probability">The probability to assign.</param>
        public virtual void SetIfLarger(T label, double probability) {
            if (!Map.ContainsKey(label) || probability <= Map[label]) 
                return;

            normalized = null;
            Map[label] = probability;
        }
        #endregion

        #region . SetLog .
        /// <summary>
        /// Assigns a log probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="label">The label to which the log probability is being assigned.</param>
        /// <param name="probability">The log probability to assign.</param>
        public virtual void SetLog(T label, double probability) {
            Set(label, Math.Exp(probability));
        }
        #endregion

        #region . Normalize .
        /// <summary>
        /// Normalizes the probabilities.
        /// </summary>
        /// <returns>The normalized probability map.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual IDictionary<T, double> Normalize() {
            var data = CreateMapDataStructure();
            var sum = Map.Sum(pair => pair.Value);

            foreach (var pair in Map)
                unchecked {
                    data[pair.Key] = pair.Value / sum;    
                }                

            return data;
        }
        #endregion
        


    }
}