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

using SharpNL.Utility;
using StringTokenizer = SharpNL.Utility.Java.StringTokenizer;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a abstract model reader.
    /// </summary>
    public abstract class AbstractModelReader : Disposable {
        protected readonly IDataReader reader;
        protected int NUM_PREDS;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModelReader"/> class.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        protected AbstractModelReader(IDataReader reader) {
            this.reader = reader;
        }

        /// <summary>
        /// Reads a <see cref="int"/> value.
        /// </summary>
        protected int ReadInt() {
            return reader.ReadInt();
        }

        /// <summary>
        /// Reads a <see cref="double"/> value.
        /// </summary>
        protected double ReadDouble() {
            return reader.ReadDouble();
        }

        /// <summary>
        /// Reads a string value.
        /// </summary>
        protected string ReadString() {
            return reader.ReadString();
        }

        /// <summary>
        /// Gets the deserialized model.
        /// </summary>
        /// <returns>A deserialized model.</returns>
        public AbstractModel GetModel() {
            CheckModelType();
            return ConstructModel();
        }

        /// <summary>
        /// Checks the type of the model.
        /// </summary>
        protected abstract void CheckModelType();

        internal abstract AbstractModel ConstructModel();

        /// <summary>
        /// Gets the outcomes.
        /// </summary>
        /// <returns>System.String[].</returns>
        protected string[] GetOutcomes() {
            var numOutcomes = ReadInt();
            var outcomeLabels = new string[numOutcomes];

            for (var i = 0; i < numOutcomes; i++)
                outcomeLabels[i] = ReadString();

            return outcomeLabels;
        }

        /// <summary>
        /// Gets the outcome patterns.
        /// </summary>
        /// <returns>System.Int32[][].</returns>
        protected int[][] GetOutcomePatterns() {
            var numOCTypes = ReadInt();
            var outcomePatterns = new int[numOCTypes][];
            for (var i = 0; i < numOCTypes; i++) {
                var tok = new StringTokenizer(ReadString(), " ");
                var infoInts = new int[tok.CountTokens];
                for (var j = 0; tok.HasMoreTokens; j++) {
                    infoInts[j] = int.Parse(tok.NextToken);
                }
                outcomePatterns[i] = infoInts;
            }
            return outcomePatterns;
        }

        /// <summary>
        /// Gets the predicates.
        /// </summary>
        /// <returns>System.String[].</returns>
        protected string[] GetPredicates() {
            NUM_PREDS = ReadInt();
            var predLabels = new string[NUM_PREDS];
            for (var i = 0; i < NUM_PREDS; i++)
                predLabels[i] = ReadString();

            return predLabels;
        }

        /// <summary>
        /// Reads the parameters from a file and populates an array of context objects.
        /// </summary>
        /// <param name="outcomePatterns">The outcomes patterns for the model. The first index refers to which
        /// outcome pattern (a set of outcomes that occurs with a context) is being specified. The
        /// second index specifies the number of contexts which use this pattern at index 0, and the
        /// index of each outcomes which make up this pattern in indices 1-n.</param>
        /// <returns>An array of context objects.</returns>
        protected Context[] GetParameters(int[][] outcomePatterns) {
            var par = new Context[NUM_PREDS];
            var pid = 0;
            foreach (var op in outcomePatterns) {
                //construct outcome pattern
                var outcomePattern = new int[op.Length - 1];
                for (var k = 1; k < op.Length; k++)
                    outcomePattern[k - 1] = op[k];

                //populate parameters for each context which uses this outcome pattern.
                for (var j = 0; j < op[0]; j++) {
                    var contextParameters = new double[op.Length - 1];
                    for (var k = 1; k < op.Length; k++)
                        contextParameters[k - 1] = ReadDouble();
                    
                    par[pid] = new Context(outcomePattern, contextParameters);
                    pid++;
                }
            }
            return par;
        }
    }
}