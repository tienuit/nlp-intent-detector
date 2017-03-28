﻿// 
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
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Class which turns a sequence stream into an event stream.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CS is using drugs! The IDisposable is implemented properly.")]
    public class SequenceStreamEventStream : Disposable, IObjectStream<Event> {
        private readonly ISequenceStream sequenceStream;
        private IEnumerator<Event> enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceStreamEventStream"/> class.
        /// </summary>
        /// <param name="sequenceStream">The sequence stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="sequenceStream"/> is null.
        /// </exception>
        public SequenceStreamEventStream(ISequenceStream sequenceStream) {
            if (sequenceStream == null)
                throw new ArgumentNullException(nameof(sequenceStream));

            this.sequenceStream = sequenceStream;
        }

        #region . DisposeManagedResources .
        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            enumerator.Dispose();

            sequenceStream?.Dispose();
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
        public Event Read() {

            if (enumerator != null && enumerator.MoveNext())
                return enumerator.Current;

            for (var sequence = sequenceStream.Read(); sequence != null; sequence = sequenceStream.Read()) {

                if (sequence.Length == 0)
                    continue;

                enumerator = new List<Event>(sequence.Events).GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current;

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
            enumerator = null;
            sequenceStream.Reset();
        }
        #endregion

    }
}