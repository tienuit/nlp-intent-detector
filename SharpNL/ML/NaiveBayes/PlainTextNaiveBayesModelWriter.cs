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
using System.IO;
using System.Text;
using SharpNL.ML.Model;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Model writer that saves models in plain text format.
    /// </summary>
    public class PlainTextNaiveBayesModelWriter : NaiveBayesModelWriter {
        private readonly PlainTextFileDataWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesModelWriter" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">outputStream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        public PlainTextNaiveBayesModelWriter(NaiveBayesModel model, Stream outputStream)
            : this(model, outputStream, Encoding.UTF8) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesModelWriter" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">outputStream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        public PlainTextNaiveBayesModelWriter(NaiveBayesModel model, Stream outputStream, Encoding encoding) : base(model) {
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));

            if (!outputStream.CanWrite)
                throw new ArgumentException("The stream is not writable.", nameof(outputStream));

            if (encoding == null)
                encoding = Encoding.UTF8;

            writer = new PlainTextFileDataWriter(outputStream, encoding);
        }

        /// <summary>
        /// Writes the specified string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void Write(string value) {
            writer.Write(value);
        }

        /// <summary>
        /// Writes the specified int value.
        /// </summary>
        /// <param name="value">The int value.</param>
        public override void Write(int value) {
            writer.Write(value);
        }

        /// <summary>
        /// Writes the specified double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public override void Write(double value) {
            writer.Write(value);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            writer.Dispose();
        }
    }
}