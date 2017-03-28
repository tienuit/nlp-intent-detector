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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;
using Sequence = SharpNL.ML.Model.Sequence;

namespace SharpNL.NameFind {

    /// <summary>
    /// Represents a <seealso cref="NameSample"/> sequence stream used to train the name finder models.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public class NameSampleSequenceStream : Disposable, ISequenceStream {
        private readonly bool useOutcomes;
        private readonly INameContextGenerator pcg;
        private readonly IObjectStream<NameSample> psi;
        private readonly ISequenceCodec<String> seqCodec;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSampleSequenceStream"/> class.
        /// </summary>
        /// <param name="psi">The sample stream.</param>
        /// <param name="featureGen">The feature generator.</param>
        public NameSampleSequenceStream(IObjectStream<NameSample> psi, IAdaptiveFeatureGenerator featureGen)
            : this(psi, new DefaultNameContextGenerator(featureGen), true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSampleSequenceStream"/> class.
        /// </summary>
        /// <param name="psi">The sample stream.</param>
        /// <param name="featureGen">The feature generator.</param>
        /// <param name="useOutcomes">if set to <c>true</c> will be used in the samples.</param>
        public NameSampleSequenceStream(IObjectStream<NameSample> psi, IAdaptiveFeatureGenerator featureGen,
            bool useOutcomes) : this(psi, new DefaultNameContextGenerator(featureGen), useOutcomes) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSampleSequenceStream"/> class.
        /// </summary>
        /// <param name="psi">The sample stream.</param>
        /// <param name="pcg">The context generator.</param>
        public NameSampleSequenceStream(IObjectStream<NameSample> psi, INameContextGenerator pcg)
            : this(psi, pcg, true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSampleSequenceStream"/> class using the <seealso cref="BioCodec"/> as sequence codec.
        /// </summary>
        /// <param name="psi">The sample stream.</param>
        /// <param name="pcg">The context generator.</param>
        /// <param name="useOutcomes">if set to <c>true</c> will be used in the samples.</param>
        public NameSampleSequenceStream(IObjectStream<NameSample> psi, INameContextGenerator pcg, bool useOutcomes)
            : this(psi, pcg, useOutcomes, new BioCodec()) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSampleSequenceStream"/> class.
        /// </summary>
        /// <param name="psi">The sample stream.</param>
        /// <param name="pcg">The context generator.</param>
        /// <param name="useOutcomes">if set to <c>true</c> will be used in the samples.</param>
        /// <param name="seqCodec">The sequence codec.</param>
        public NameSampleSequenceStream(IObjectStream<NameSample> psi, INameContextGenerator pcg, bool useOutcomes,
            ISequenceCodec<string> seqCodec) {
            this.psi = psi;
            this.useOutcomes = useOutcomes;
            this.pcg = pcg;
            this.seqCodec = seqCodec;
        }

        #endregion

        #region . DisposeManagedResources .

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            psi.Dispose();
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public Sequence Read() {
            var sample = psi.Read();
            if (sample != null) {

                var events = new Event[sample.Sentence.Length];

                for (int i = 0; i < sample.Sentence.Length; i++) {

                    // it is safe to pass the tags as previous tags because
                    // the context generator does not look for non predicted tags
                    var tags = seqCodec.Encode(sample.Names, sample.Sentence.Length);


                    var context = pcg.GetContext(
                        i,
                        sample.Sentence,
                        useOutcomes ? tags : null,
                        null);

                    events[i] = new Event(tags[i], context);
                }

                return new Sequence(events, sample);
            }
            return null;
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            psi.Reset();
        }
        #endregion

        #region . UpdateContext .
        /// <summary>
        /// Creates a new event array based on the outcomes predicted by the specified parameters for the specified sequence.
        /// </summary>
        /// <param name="sequence">The sequence to be evaluated.</param>
        /// <param name="model">The model.</param>
        /// <returns>The event array.</returns>
        public Event[] UpdateContext(Sequence sequence, AbstractModel model) {
            var tagger =
                new NameFinderME(
                    new TokenNameFinderModel("x-unspecified", model, new Dictionary<string, object>(), null));

            var sentence = sequence.GetSource<NameSample>().Sentence;

            var tags = seqCodec.Encode(tagger.Find(sentence), sentence.Length);

            return NameFinderEventStream.GenerateEvents(sentence, tags, pcg).ToArray();
        }
        #endregion

    }
}