// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
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

using SharpNL.Utility;

namespace SharpNL.ML {
    /// <summary>
    /// The interface for sequence classification models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISequenceClassificationModel<T> {

        /// <summary>
        /// Finds the sequence with the highest probability.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="additionalContext"></param>
        /// <param name="beamSearch"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        Sequence BestSequence(
            T[] sequence,
            object[] additionalContext,
            IBeamSearchContextGenerator<T> beamSearch,
            ISequenceValidator<T> validator);


        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="minSequenceScore">The minimum sequence score.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        Sequence[] BestSequences(
            int numSequences,
            T[] sequence,
            object[] additionalContext,
            double minSequenceScore,
            IBeamSearchContextGenerator<T> beamSearch,
            ISequenceValidator<T> validator);


        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        Sequence[] BestSequences(
            int numSequences,
            T[] sequence,
            object[] additionalContext,
            IBeamSearchContextGenerator<T> beamSearch,
            ISequenceValidator<T> validator);


        /// <summary>
        /// Gets all possible outcomes.
        /// </summary>
        /// <returns>all possible outcomes.</returns>
        string[] GetOutcomes();
    }
}