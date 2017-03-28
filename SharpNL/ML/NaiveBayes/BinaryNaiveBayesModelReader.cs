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
    /// Represents a binary model reader for <see cref="NaiveBayesModel"/> models.
    /// </summary>
    public class BinaryNaiveBayesModelReader : NaiveBayesModelReader {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveBayesModelReader"/> class.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        public BinaryNaiveBayesModelReader(IDataReader reader) : base(reader) {

        }

        /// <summary>
        /// Constructor which directly instantiates the <see cref="Stream"/> containing the model contents.
        /// </summary>
        /// <param name="inputStream">The input stream containing the model information.</param>
        public BinaryNaiveBayesModelReader(Stream inputStream) : base(new BinaryFileDataReader(inputStream)) {

        }


    }
}