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
using System.IO;
using System.Text;
using SharpNL.Utility;
using StringTokenizer = SharpNL.Utility.Java.StringTokenizer;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Class for using a file of events as an event stream. The format of the file is one event per line with
    /// each line consisting of outcome followed by contexts (space delimited).
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public class FileEventStream : Disposable, IObjectStream<Event> {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventStream"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="inputStream"/></exception>
        /// <exception cref="System.ArgumentException">The stream is not readable.</exception>
        public FileEventStream(Stream inputStream)
            : this(inputStream, Encoding.UTF8) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventStream"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/>
        /// or
        /// <paramref name="encoding"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The stream is not readable.</exception>
        public FileEventStream(Stream inputStream, Encoding encoding) {
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream));
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"The stream is not readable.", nameof(inputStream));
            }

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            reader = new StreamReader(inputStream, encoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventStream"/> class using a file as source.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public FileEventStream(string fileName) : this(fileName, Encoding.UTF8) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventStream"/> class using a file as source with the specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        public FileEventStream(string fileName, Encoding encoding) {
            reader = new StreamReader(fileName, encoding);
        }

        #endregion

        #region + Properties .

        #region . Reader .

        private readonly StreamReader reader;

        /// <summary>
        /// Gets the stream reader.
        /// </summary>
        /// <value>The stream reader.</value>
        protected StreamReader Reader => reader;

        #endregion

        #endregion

        #region . Dispose .

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();
        
            reader.Dispose();
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public virtual Event Read() {
            string line = Reader.ReadLine();

            if (line == null) {
                return null;
            }

            var st = new StringTokenizer(line);
            string outcome = st.NextToken;
            var count = st.CountTokens;
            var context = new string[count];

            for (int i = 0; i < count; i++) {
                context[i] = st.NextToken;
            }
            return new Event(outcome, context);
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual void Reset() {
            throw new NotSupportedException();
        }
        #endregion

        #region . ToLine .
        /// <summary>
        /// Generates a string representing the specified event.
        /// </summary>
        /// <param name="ev">The event for which a string representation is needed.</param>
        /// <returns>A string representing the specified event.</returns>
        /// <exception cref="System.ArgumentNullException">ev</exception>
        public static string ToLine(Event ev) {
            if (ev == null) {
                throw new ArgumentNullException(nameof(ev));
            }
            return $"{ev.Outcome} {string.Join(" ", ev.Context)}{Environment.NewLine}";
        }
        #endregion

    }
}