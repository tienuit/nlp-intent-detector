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

namespace SharpNL.ML.NaiveBayes {
    public class LogProbability<T> : Probability<T> {
        /// <summary>
        /// Initializes a new instance of the <see cref="Probability{T}"/> class.
        /// </summary>
        /// <param name="label">The probability label.</param>
        public LogProbability(T label) : base(label) {
            // set(1) = log(1) = 0 
        }

        /// <summary>
        /// Gets or sets the probability value.
        /// </summary>
        /// <value>The probability value.</value>
        public override double Value => Math.Exp(base.Value);

        /// <summary>
        /// Gets the log probability associated with a label.
        /// </summary>
        /// <value>The log probability associated with a label.</value>
        public override double Log => base.Value;

        /// <summary>
        /// Assigns a log probability to a label, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The log probability to assign.</param>
        public override void SetLog(double probability) {
            base.Set(probability);
        }

        /// <summary>
        /// Compounds the existing probability mass on the label with the new probability passed in to the method.
        /// </summary>
        /// <param name="probability">The probability weight to add.</param>
        public override void AddIn(double probability) {
            base.Set(base.Value + Math.Log(probability));
        }

        /// <summary>
        /// Determines whether the specified probability is larger than the current probability.
        /// </summary>
        /// <param name="probability">The probability to check.</param>
        /// <returns><c>true</c> if the specified probability is larger than the current probability; otherwise, <c>false</c>.</returns>
        public override bool IsLarger(double probability) {
            return base.IsLarger(Math.Log(probability));
        }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public override void Set(double probability) {
            base.Set(Math.Log(probability));
        }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public override void Set(IProbability probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));

            base.Set(probability.Log);
        }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public override void SetIfLarger(double probability) {
            base.SetIfLarger(Math.Log(probability));
        }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        public override void SetIfLarger(IProbability probability) {
            if (probability == null)
                throw new ArgumentNullException(nameof(probability));

            base.SetIfLarger(probability.Log);
        }

        /// <summary>
        /// Determines whether the specified probability is larger than the current probability.
        /// </summary>
        /// <param name="probability">The probability to check.</param>
        /// <returns><c>true</c> if the specified probability is larger than the current probability; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">probability</exception>
        public override bool IsLarger(IProbability probability) {
            return base.IsLarger(probability.Log);
        }
    }
}