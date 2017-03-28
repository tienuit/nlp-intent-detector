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
using System.Runtime.Serialization;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// Represents errors that occur with feature generators.
    /// </summary>
    [TypeClass("opennlp.tools.util.featuregen.FeatureGeneratorException")]
    [Serializable]
    public class FeatureGeneratorException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FeatureGeneratorException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public FeatureGeneratorException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FeatureGeneratorException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param><param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public FeatureGeneratorException(string message, Exception innerException) : base(message, innerException) {}

    }
}