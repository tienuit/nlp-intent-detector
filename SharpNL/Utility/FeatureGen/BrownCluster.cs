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
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpNL.Utility.Serialization;

namespace SharpNL.Utility.FeatureGen {

    /// <summary>
    /// Class to load a Brown cluster document: word\tword_class\tprob
    /// <see href="http://metaoptimize.com/projects/wordreprs/"/>.
    /// 
    /// The file containing the clustering lexicon has to be passed as the
    /// value of the dict attribute of each BrownCluster feature generator.
    /// </summary>
    public sealed class BrownCluster {
        private readonly Dictionary<string, string> tokenToClusterMap;

        #region + Constructors .
        /// <summary>
        /// Generates the token to cluster map from Brown cluster input stream using the minimum token frequency as 5.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="System.ArgumentException">@The stream is not readable.;inputStream</exception>
        public BrownCluster(Stream inputStream)
            : this(inputStream, 5) {

        }
        /// <summary>
        /// Generates the token to cluster map from Brown cluster input stream with a minimum token frequency.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="minFrequency">The minimum frequency.</param>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="System.ArgumentException">@The stream is not readable.;inputStream</exception>
        public BrownCluster(Stream inputStream, int minFrequency) {

            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (inputStream.CanRead)
                throw new ArgumentException(@"The stream is not readable.", nameof(inputStream));

            tokenToClusterMap = new Dictionary<string, string>();

            using (var reader = new StreamReader(inputStream, Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {

                    var tokens = line.Split('\t');

                    int count;
                    if (tokens.Length == 3 && int.TryParse(tokens[2], out count) && count > minFrequency)
                        tokenToClusterMap[tokens[1]] = tokens[0];
                    else if (tokens.Length == 2)
                        tokenToClusterMap[tokens[0]] = tokens[1];

                }
            }
        }
        #endregion

        /// <summary>
        /// Gets the <see cref="string"/> with the specified key.
        /// </summary>
        /// <param name="key">The token to look-up.</param>
        /// <returns>The brown class if such token is in the brown cluster map.</returns>
        public string this[string key] => tokenToClusterMap[key];

        internal static void Serialize(object artifact, Stream outputStream) {

            var brownCluster = artifact as BrownCluster;
            if (brownCluster == null)
                throw new InvalidOperationException();

            using (var writer = new StreamWriter(outputStream, Encoding.UTF8, 1024, true)) {
                foreach (var pair in brownCluster.tokenToClusterMap) {
                    writer.WriteLine("{0}\t{1}\n", pair.Key, pair.Value);
                }
                writer.Flush();
            }
        }

        internal static object Deserialize(Stream inputStream) {
            return new BrownCluster(inputStream);
        }

    }



}