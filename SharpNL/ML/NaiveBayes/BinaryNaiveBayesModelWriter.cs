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

using System.IO;
using SharpNL.ML.Model;

namespace SharpNL.ML.NaiveBayes {
    /// <summary>
    /// Model writer that saves <see cref="NaiveBayesModel"/> model in binary format.
    /// </summary>
    public class BinaryNaiveBayesModelWriter : NaiveBayesModelWriter {
        private readonly BinaryFileDataWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryNaiveBayesModelWriter"/> class.
        /// </summary>
        /// <param name="model">The <see cref="NaiveBayesModel"/> which is to be persisted.</param>
        /// <param name="outputStream">The stream which will be used to persist the model.</param>
        public BinaryNaiveBayesModelWriter(AbstractModel model, Stream outputStream)
            : base(model) {

            writer = new BinaryFileDataWriter(outputStream);
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
            writer.Flush();
            writer.Close();
        }
    }
}