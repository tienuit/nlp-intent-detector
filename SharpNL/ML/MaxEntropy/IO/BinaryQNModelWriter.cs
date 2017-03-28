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

using System.IO;

namespace SharpNL.ML.MaxEntropy.IO {
    using Model;
    public class BinaryQNModelWriter : QNModelWriter {
        private readonly BinaryFileDataWriter writer;

        /// <summary>
        /// Constructor which takes a <see cref="GISModel"/> and a <paramref name="outStream"/> and prepares itself to write the model to that stream.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="outStream">The out stream.</param>
        public BinaryQNModelWriter(AbstractModel model, Stream outStream) : base(model) {
            writer = new BinaryFileDataWriter(outStream);
        }

        public override void Write(string value) {
            writer.Write(value);
        }

        public override void Write(int value) {
            writer.Write(value);
        }

        public override void Write(double value) {
            writer.Write(value);
        }

        public override void Close() {
            writer.Flush();
            writer.Close();
        }
    }
}