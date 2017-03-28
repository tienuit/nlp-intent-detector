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
    /// Represents a probability distribution over labels returned by a classifier as a log of probabilities.
    /// </summary>
    /// <typeparam name="T">The label type.</typeparam>
    public class LogProbabilities<T> : Probabilities<T> {

        #region . AddIn .
        /// <summary>
        /// Compounds the existing probability mass on the label with the new probability passed in to the method.
        /// </summary>
        /// <param name="label">The label whose probability mass is being updated.</param>
        /// <param name="probability">The probability weight to add.</param>
        /// <param name="count">The amplifying factor for the probability compounding.</param>
        public override void AddIn(T label, double probability, int count) {
            Normalized = null;
            var p = Map.ContainsKey(label) ? Map[label] : 0d;

            probability = Math.Log(probability)*count;
            Map[label] = p + probability;
        }
        #endregion

        #region . Get .
        /// <summary>
        /// Gets the probability associated with a label.
        /// </summary>
        /// <param name="label">The label whose probability needs to be returned.</param>
        /// <returns>The probability associated with the label.</returns>
        public override double Get(T label) {
            return !Normalized.ContainsKey(label) ? 0d : Normalized[label];
        }
        #endregion

        #region . GetLog .

        /// <summary>
        /// Gets the log probability associated with a label.
        /// </summary>
        /// <param name="label">The label whose log probability needs to be returned.</param>
        /// <returns>The log probability associated with the label.</returns>
        public override double GetLog(T label) {
            return Map.ContainsKey(label) ? Map[label] : double.NegativeInfinity;
        }

        #endregion

        #region . Normalize .
        /// <summary>
        /// Normalizes the probabilities.
        /// </summary>
        /// <returns>The normalized probability map.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override IDictionary<T, double> Normalize() {
            var data = CreateMapDataStructure();

            var highestLogProbability = double.NegativeInfinity;
            foreach (var pair in Map) {
                if (pair.Value > highestLogProbability)
                    highestLogProbability = pair.Value;
            }

            var sum = 0d;
            foreach (var pair in Map) {
                var p = Math.Exp(pair.Value - highestLogProbability);
                if (double.IsNaN(p))
                    continue;

                sum += p;
                data[pair.Key] = p;
            }

            if (sum > double.MinValue)
                foreach (var key in data.Keys.ToList())
                    data[key] = data[key]/sum;

            return data;

        }
        #endregion

        #region + Set .
        /// <summary>
        /// Assigns a probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="label">The label to which the probability is being assigned.</param>
        /// <param name="probability">The probability to assign.</param>
        public override void Set(T label, double probability) {
            base.Set(label, Math.Log(probability));
        }

        /// <summary>
        /// Assigns a probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="label">The label to which the probability is being assigned.</param>
        /// <param name="probability">The probability to assign.</param>
        /// <exception cref="ArgumentNullException">probability</exception>
        public override void Set(T label, Probability<T> probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));

            base.Set(label, probability.Log);
        }
        #endregion

        #region . SetIfLarger .
        /// <summary>
        /// Assigns a probability to a label, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="label">The label to which the probability is being assigned.</param>
        /// <param name="probability">The probability to assign.</param>
        public override void SetIfLarger(T label, double probability) {
            base.SetIfLarger(label, Math.Log(probability));
        }
        #endregion

    }
}