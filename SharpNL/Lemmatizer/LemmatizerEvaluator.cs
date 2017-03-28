//  
//  Copyright 2016 Gustavo J Knuppe (https://github.com/knuppe)
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
using SharpNL.Utility.Evaluation;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// The lemmatizer evaluator measures the performance of the given <see cref="ILemmatizer" /> with the provided reference
    /// <see cref="LemmaSample" />s.
    /// </summary>
    public class LemmatizerEvaluator : Evaluator<LemmaSample, LemmaSample> {
        private readonly Mean accuracy = new Mean();

        private readonly ILemmatizer lemmatizer;

        /// <summary>
        /// Initializes the current instance.
        /// </summary>
        /// <param name="lemmatizer">A lemmatizer.</param>
        /// <param name="listeners">An array of evaluation listeners.</param>
        public LemmatizerEvaluator(ILemmatizer lemmatizer, params IEvaluationMonitor<LemmaSample>[] listeners) : base(listeners) {
            if (lemmatizer == null)
                throw new ArgumentNullException(nameof(lemmatizer));

            this.lemmatizer = lemmatizer;
        }

        /// <summary>
        /// Gets the word accuracy.
        /// </summary>
        /// <remarks>
        /// This is defined as: word accuracy = correctly detected tags / total words
        /// </remarks>
        public double WordAccuracy => accuracy.Value;

        /// <summary>
        /// Gets the total number of words considered in the evaluation.
        /// </summary>
        public long WordCount => accuracy.Count;

        protected override LemmaSample ProcessSample(LemmaSample reference) {
            var predictedLemmas = lemmatizer.Lemmatize(reference.Tokens, reference.Tags);
            var referenceLemmas = reference.Lemmas;

            for (var i = 0; i < referenceLemmas.Length; i++)
                accuracy.Add(referenceLemmas[i].Equals(predictedLemmas[i]) ? 1 : 0);

            return new LemmaSample(reference.Tokens, reference.Tags, predictedLemmas);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() {
            return $"Accuracy: {WordAccuracy} Number of Samples: {WordCount}";
        }
    }
}