﻿// 
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

using System.Xml;

namespace SharpNL.Utility.FeatureGen.Factories {
    /// <summary>
    /// Generates Brown clustering features for token classes.
    /// </summary>
    internal class BrownClusterTokenClassFeatureGeneratorFactory : XmlFeatureGeneratorFactory {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrownClusterTokenClassFeatureGeneratorFactory"/> with the unique factory name.
        /// </summary>
        public BrownClusterTokenClassFeatureGeneratorFactory() : base("brownclustertokenclass") { }

        /// <summary>
        /// Creates an <see cref="IAdaptiveFeatureGenerator"/> from a the describing XML element.
        /// </summary>
        /// <param name="generatorElement">The element which contains the configuration.</param>
        /// <param name="provider">The resource provider which is used to resolve resources referenced by a key in the descriptor.</param>
        /// <returns>The configured <see cref="IAdaptiveFeatureGenerator"/> </returns>
        public override IAdaptiveFeatureGenerator Create(XmlElement generatorElement,
            FeatureGeneratorResourceProvider provider) {
            var dictResourceKey = generatorElement.GetAttribute("dict");
            var dictResource = provider(dictResourceKey);

            if (!(dictResource is BrownCluster))
                throw new InvalidFormatException("Not a BrownLexicon resource for key: " + dictResourceKey);


            return new BrownTokenClassFeatureGenerator((BrownCluster) dictResource);
        }
    }
}