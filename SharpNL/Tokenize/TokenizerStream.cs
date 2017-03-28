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
using System.Diagnostics.CodeAnalysis;
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// The <see cref="TokenizerStream"/> uses a tokenizer to tokenize the input string and output <see cref="TokenSample"/>s.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public class TokenizerStream : Disposable, IObjectStream<TokenSample> {

        private readonly ITokenizer tokenizer;
        private readonly IObjectStream<string> input;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerStream"/> class.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="input">The input stream object.</param>
        public TokenizerStream(ITokenizer tokenizer, IObjectStream<string> input) {
            this.tokenizer = tokenizer;
            this.input = input;
        }
        #endregion

        #region . DisposeManagedResources .
        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            input.Dispose();
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
        public TokenSample Read() {

            var inputString = input.Read();

            if (inputString != null) {
                var tokens = tokenizer.TokenizePos(inputString);
                return new TokenSample(inputString, tokens);
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
        /// <exception cref="NotSupportedException"></exception>
        public void Reset() {
            input.Reset();
        }
        #endregion

    }
}