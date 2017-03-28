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
using System.Diagnostics.CodeAnalysis;
using SharpNL.ML.Model;
using SharpNL.Utility;
using Sequence = SharpNL.ML.Model.Sequence;

namespace SharpNL.Chunker {
    /// <summary>
    /// Represents a <see cref="ChunkSample" /> sequence stream.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs again! The interface is properly implemented.")]
    public sealed class ChunkSampleSequenceStream : Disposable, ISequenceStream {
        private readonly IChunkerContextGenerator contextGenerator;
        private readonly IObjectStream<ChunkSample> samples;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkSampleSequenceStream" /> class using the given parameters.
        /// </summary>
        /// <param name="samples">The chunk samples.</param>
        /// <param name="contextGenerator">The chunker context generator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="samples"/> is null.
        /// or
        /// The <paramref name="contextGenerator"/> is null.
        /// </exception>
        public ChunkSampleSequenceStream(IObjectStream<ChunkSample> samples, IChunkerContextGenerator contextGenerator) {
            if (samples == null)
                throw new ArgumentNullException(nameof(samples));

            if (contextGenerator == null)
                throw new ArgumentNullException(nameof(contextGenerator));

            this.samples = samples;
            this.contextGenerator = contextGenerator;
        }
        #endregion

        #region . DisposeManagedResources .

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            samples.Dispose();
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next <see cref="Sequence"/>. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public Sequence Read() {
            var sample = samples.Read();
            if (sample == null) 
                return null;

            var events = new Event[sample.Sentence.Count];
            for (int i = 0, count = sample.Sentence.Count; i < count; i++) {
                events[i] = new Event(
                    sample.Tags[i],
                    // it is safe to pass the tags as previous tags because
                    // the context generator does not look for non predicted tags
                    contextGenerator.GetContext(i, sample.Sentence.ToArray(), sample.Tags.ToArray(), null));
            }
            return new Sequence(events, sample);
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            samples.Reset();
        }

        #endregion

        #region . UpdateContext .

        /// <summary>
        /// Creates a new event array based on the outcomes predicted by the specified parameters for the specified sequence.
        /// </summary>
        /// <param name="sequence">The sequence to be evaluated.</param>
        /// <param name="model">The model.</param>
        /// <returns>The event array.</returns>
        /// <remarks>Always return null.</remarks>
        public Event[] UpdateContext(Sequence sequence, AbstractModel model) {
            // TODO: Should be implemented for Perceptron sequence learning ...
            return null;
        }

        #endregion
    }
}