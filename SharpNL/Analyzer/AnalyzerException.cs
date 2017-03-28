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
using System.Security.Permissions;

namespace SharpNL.Analyzer {
    /// <summary>
    /// Represents a exception that occurs in a <see cref="IAnalyzer"/> object.
    /// </summary>
    [Serializable]
    public class AnalyzerException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AnalyzerException"/> class.
        /// </summary>
        public AnalyzerException(IAnalyzer analyzer) {
            Analyzer = analyzer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="analyzer">The analyzer which the exception occurred.</param>
        /// <param name="message">The message that describes the error. </param>
        public AnalyzerException(IAnalyzer analyzer, string message) : base(message) {
            Analyzer = analyzer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AnalyzerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="analyzer">The analyzer which the exception occurred.</param>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public AnalyzerException(IAnalyzer analyzer, string message, Exception innerException)
            : base(message, innerException) {
            Analyzer = analyzer;
        }

        #region . Analyzer .
        /// <summary>
        /// Gets the analyzer which the exception occurred.
        /// </summary>
        /// <value>The analyzer which the exception occurred.</value>
        public IAnalyzer Analyzer { get; private set; }
        #endregion

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">info</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
        }

    }
}