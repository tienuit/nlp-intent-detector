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
using SharpNL.Utility;

namespace SharpNL.Formats.Ptb {
    /// <summary>
    /// Base class for Penn Treebank sample streams.
    /// </summary>
    /// <typeparam name="T">The sample type.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public abstract class PtbSampleStream<T> : Disposable, IObjectStream<T> {

        private readonly bool ownsStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSampleStream{T}"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        protected PtbSampleStream(PtbStreamReader stream) : this(stream, true) {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSampleStream{T}"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="ownsStream"><c>true</c> to indicate that the stream will be disposed when this stream is disposed; <c>false</c> to indicate that the stream will not be disposed when this stream is disposed.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        protected PtbSampleStream(PtbStreamReader stream, bool ownsStream) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Stream = stream;

            this.ownsStream = ownsStream;
        }

        #region . Stream .
        /// <summary>
        /// Gets the Penn Treebank stream.
        /// </summary>
        /// <value>The Penn Treebank stream.</value>
        protected PtbStreamReader Stream { get; private set; }
        #endregion

        #region . DisposeManagedResources .
        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            if (ownsStream && Stream != null)
                Stream.Dispose();

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
        public abstract T Read();
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public virtual void Reset() {
            Stream.Reset();
        }
        #endregion

    }
}