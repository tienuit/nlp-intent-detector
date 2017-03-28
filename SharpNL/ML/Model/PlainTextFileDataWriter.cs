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
using System.Globalization;
using System.IO;
using System.Text;
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a plain text data writer.
    /// </summary>
    public class PlainTextFileDataWriter : Disposable, IDataWriter {

        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextFileDataWriter"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        public PlainTextFileDataWriter(Stream outputStream) : this(outputStream, Encoding.UTF8) {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextFileDataWriter"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <exception cref="System.ArgumentNullException">outputStream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.;outputStream</exception>
        public PlainTextFileDataWriter(Stream outputStream, Encoding encoding) {
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));

            if (!outputStream.CanWrite)
                throw new ArgumentException("The stream is not writable.", nameof(outputStream));

            if (encoding == null)
                encoding = Encoding.UTF8;

            writer = new StreamWriter(outputStream, encoding) {
                AutoFlush = true
            };

        }

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            writer.Dispose();
        }

        /// <summary>
        /// Writes the specified string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public void Write(string value) {
            writer.WriteLine(value);
        }

        /// <summary>
        /// Writes the specified double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public void Write(double value) {
            writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the specified integer value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public void Write(int value) {
            writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}