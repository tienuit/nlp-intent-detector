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
using System.Linq;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// Generates Brown cluster features for current token.
    /// </summary>
    public class BrownTokenFeatureGenerator : FeatureGeneratorAdapter {
        private readonly BrownCluster brownLexicon;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrownTokenFeatureGenerator"/> class.
        /// </summary>
        /// <param name="brownLexicon">The Brown lexicon.</param>
        /// <exception cref="System.ArgumentNullException">brownLexicon</exception>
        public BrownTokenFeatureGenerator(BrownCluster brownLexicon) {
            if (brownLexicon == null)
                throw new ArgumentNullException(nameof(brownLexicon));

            this.brownLexicon = brownLexicon;
        }

        /// <summary>
        /// Adds the appropriate features for the token at the specified index with the
        /// specified array of previous outcomes to the specified list of features.
        /// </summary>
        /// <param name="features">The list of features to be added to.</param>
        /// <param name="tokens">The tokens of the sentence or other text unit being processed.</param>
        /// <param name="index">The index of the token which is currently being processed.</param>
        /// <param name="previousOutcomes">The outcomes for the tokens prior to the specified index.</param>
        public override void CreateFeatures(List<string> features, string[] tokens, int index, string[] previousOutcomes) {
            var wordClasses = BrownTokenClasses.GetWordClasses(tokens[index], brownLexicon);

            features.AddRange(wordClasses.Select(wordClass => "browncluster=" + wordClass));
        }
    }
}