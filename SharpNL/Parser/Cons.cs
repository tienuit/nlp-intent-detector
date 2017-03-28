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

namespace SharpNL.Parser {
    /// <summary>
    /// Holds feature information about a specific parse node.
    /// </summary>
    public struct Cons {
        public readonly string cons;
        public readonly string consbo;
        public readonly int index;
        public readonly bool unigram;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cons"/> struct.
        /// </summary>
        /// <param name="cons">The cons.</param>
        /// <param name="consbo">The consbo.</param>
        /// <param name="index">The index.</param>
        /// <param name="unigram">if set to <c>true</c> [unigram].</param>
        public Cons(string cons, string consbo, int index, bool unigram) {
            this.cons = cons;
            this.consbo = consbo;
            this.index = index;
            this.unigram = unigram;
        }
    }
}