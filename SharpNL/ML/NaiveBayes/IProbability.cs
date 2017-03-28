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
namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Represents a probability value.
    /// </summary>
    public interface IProbability {

        /// <summary>
        /// Gets the probability value.
        /// </summary>
        /// <value>The probability value.</value>
        double Value { get; }

        double Log { get; }

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        void Set(double probability);

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        void Set(IProbability probability);


        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        void SetIfLarger(double probability);

        /// <summary>
        /// Sets the specified probability value, discarding any previously assigned probability, if the new probability is greater than the old one.
        /// </summary>
        /// <param name="probability">The probability to assign.</param>
        void SetIfLarger(IProbability probability);

    }
}