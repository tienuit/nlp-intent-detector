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
using System.Collections.Generic;
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.Utility;
using Sequence = SharpNL.Utility.Sequence;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents a maximum-entropy-based lemmatizer.
    /// </summary>
    /// <seealso cref="ILemmatizer" />
    public class LemmatizerME : ILemmatizer {
        public const int DefaultBeamSize = 3;
        private readonly ILemmatizerContextGenerator contextGenerator;

        private readonly ML.Model.ISequenceClassificationModel<string> model;
        private readonly ISequenceValidator<string> sequenceValidator;
        private Sequence bestSequence;


        public LemmatizerME(LemmatizerModel model) {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            contextGenerator = model.Factory.GetContextGenerator();
            sequenceValidator = model.Factory.GetSequenceValidator();

            // Knuppe: In the original implementation there is condition to recreate the beamsearch object, but 
            // the condition is impossible to occur, due to the getLemmatizerSequenceModel() method logic
            this.model = model.LemmatizerSequenceModel;
        }

        /// <summary>
        /// Gets the probabilities of the last decoded sequence.
        /// </summary>
        /// <returns>
        /// An array with the same number of probabilities as tokens were sent to <see cref="Lemmatize" /> when it was
        /// last called.
        /// </returns>
        /// <remarks>The sequence was determined based on the previous call to <see cref="Lemmatize" />.</remarks>
        public double[] Probabilities => bestSequence == null ? null : bestSequence.Probabilities.ToArray();

        #region . Lemmatize .

        /// <summary>
        /// Returns the lemma of the specified word with the specified part-of-speech.
        /// </summary>
        /// <param name="tokens">An array of the tokens.</param>
        /// <param name="tags">An array of the POS tags.</param>
        /// <returns>An array of lemma classes for each token in the sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tokens" /> or <paramref name="tags" /></exception>
        /// <exception cref="ArgumentException">The arguments must have the same length.</exception>
        public string[] Lemmatize(string[] tokens, string[] tags) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            if (tokens.Length != tags.Length)
                throw new ArgumentException("The arguments must have the same length.");

            bestSequence = model.BestSequence(tokens, new object[] {tags}, contextGenerator, sequenceValidator);

            return bestSequence.Outcomes.ToArray();
        }

        #endregion

        #region . DecodeLemmas .

        /// <summary>
        /// Decodes the lemma from the word and the induced lemma class.
        /// </summary>
        /// <param name="tokens">The array of token</param>
        /// <param name="preds">The predicted lemma classes.</param>
        /// <returns>An array of decoded lemmas.</returns>
        public string[] DecodeLemmas(string[] tokens, string[] preds) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (preds == null)
                throw new ArgumentNullException(nameof(preds));

            if (tokens.Length != preds.Length)
                throw new ArgumentException("The arguments must have the same length.");

            var lemmas = new List<string>(tokens.Length);

            for (var i = 0; i < tokens.Length; i++) {
                var lemma = LemmatizerUtils.DecodeShortestEditScript(tokens[i].ToLowerInvariant(), preds[i]);

                if (string.IsNullOrEmpty(lemma))
                    lemma = "_";

                lemmas.Add(lemma);
            }

            return lemmas.ToArray();
        }

        #endregion

        #region + TopKSequences .

        /// <summary>
        /// Returns the top k sequences for the specified tokens with the specified pos-tags.
        /// </summary>
        /// <param name="tokens">The tokens of the tokens.</param>
        /// <param name="tags">The pos-tags for the specified tokens.</param>
        /// <returns>The top k sequences for the specified tokens.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tokens" /> or <paramref name="tags" /></exception>
        /// <exception cref="ArgumentException">The arguments must have the same length.</exception>
        public Sequence[] TopKSequences(string[] tokens, string[] tags) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            if (tokens.Length != tags.Length)
                throw new ArgumentException("The arguments must have the same length.");


            return model.BestSequences(DefaultBeamSize, tokens, new object[] {tags}, contextGenerator, sequenceValidator);
        }

        /// <summary>
        /// Returns the top k sequences for the specified tokens with the specified pos-tags.
        /// </summary>
        /// <param name="tokens">The tokens of the tokens.</param>
        /// <param name="tags">The pos-tags for the specified tokens.</param>
        /// <param name="minScore">A lower bound on the score of a returned sequence.</param>
        /// <returns>The top k sequences for the specified tokens.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tokens" /> or <paramref name="tags" /></exception>
        /// <exception cref="ArgumentException">The arguments must have the same length.</exception>
        public Sequence[] TopKSequences(string[] tokens, string[] tags, double minScore) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            if (tokens.Length != tags.Length)
                throw new ArgumentException("The arguments must have the same length.");

            return model.BestSequences(DefaultBeamSize, tokens, new object[] {tags}, minScore, contextGenerator, sequenceValidator);
        }

        #endregion

        #region . Train .

        /// <summary>
        /// Trains a lemmatizer model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="factory">The sentence detector factory.</param>
        /// <returns>The trained <see cref="LemmatizerModel" /> object.</returns>
        /// <exception cref="System.InvalidOperationException">The trainer was not specified.</exception>
        /// <exception cref="System.NotSupportedException">Trainer type is not supported.</exception>
        public static LemmatizerModel Train(string languageCode, IObjectStream<LemmaSample> samples, TrainingParameters parameters, LemmatizerFactory factory) {
            return Train(languageCode, samples, parameters, factory, null);
        }

        /// <summary>
        /// Trains a lemmatizer model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="factory">The sentence detector factory.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training
        /// operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The trained <see cref="LemmatizerModel" /> object.</returns>
        /// <exception cref="System.InvalidOperationException">The trainer was not specified.</exception>
        /// <exception cref="System.NotSupportedException">Trainer type is not supported.</exception>
        public static LemmatizerModel Train(string languageCode, IObjectStream<LemmaSample> samples, TrainingParameters parameters, LemmatizerFactory factory, Monitor monitor) {
            var manifestInfoEntries = new Dictionary<string, string>();
            var beamSize = parameters.Get(Parameters.BeamSize, DefaultBeamSize);
            var cg = factory.GetContextGenerator();


            var trainerType = TrainerFactory.GetTrainerType(parameters);
            if (!trainerType.HasValue)
                throw new InvalidOperationException("The trainer was not specified.");


            IMaxentModel model = null;
            ML.Model.ISequenceClassificationModel<string> seqModel = null;

            switch (trainerType) {
                case TrainerType.EventModelTrainer:
                    var s1 = new LemmaSampleEventStream(samples, cg);
                    var t1 = TrainerFactory.GetEventTrainer(parameters, manifestInfoEntries, monitor);

                    model = t1.Train(s1);
                    break;
                case TrainerType.EventModelSequenceTrainer:
                    var s2 = new LemmaSampleSequenceStream(samples, cg);
                    var t2 = TrainerFactory.GetEventModelSequenceTrainer(parameters, manifestInfoEntries, monitor);

                    model = t2.Train(s2);
                    break;
                case TrainerType.SequenceTrainer:
                    var s3 = new LemmaSampleSequenceStream(samples, cg);
                    var t3 = TrainerFactory.GetSequenceModelTrainer(parameters, manifestInfoEntries, monitor);

                    seqModel = t3.Train(s3);
                    break;
                default:
                    throw new NotSupportedException("Trainer type is not supported.");
            }

            return model != null
                ? new LemmatizerModel(languageCode, model, beamSize, manifestInfoEntries, factory)
                : new LemmatizerModel(languageCode, seqModel, manifestInfoEntries, factory);
        }

        #endregion
    }
}