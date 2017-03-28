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
using System.Linq;
using System.Xml;

namespace SharpNL.Utility.FeatureGen.Factories {
    // TODO: We have to support custom resources here. How does it work ?!
    // Attributes get into a Map<String, String> properties

    // How can serialization be supported ?!
    // The model is loaded, and the manifest should contain all serializer classes registered for the
    // resources by name.
    // When training, the descriptor could be consulted first to register the serializers, and afterwards
    // they are stored in the model.
    [TypeClass("opennlp.tools.util.featuregen.GeneratorFactory.CustomFeatureGeneratorFactory")]
    public class CustomFeatureGeneratorFactory : XmlFeatureGeneratorFactory {

        public CustomFeatureGeneratorFactory() : base("custom") {}

        /// <summary>
        /// Creates an <see cref="IAdaptiveFeatureGenerator"/> from a the describing XML element.
        /// </summary>
        /// <param name="generatorElement">The element which contains the configuration.</param>
        /// <param name="provider">The resource provider which could be used to access referenced resources.</param>
        /// <returns>The configured <see cref="IAdaptiveFeatureGenerator"/> </returns>
        public override IAdaptiveFeatureGenerator Create(XmlElement generatorElement, FeatureGeneratorResourceProvider provider) {
            var className = generatorElement.GetAttribute("class");

            if (!Library.TypeResolver.IsRegistered(className))
                throw new NotSupportedException("The class " + className + " is not registered on the TypeResolver.");

            var type = Library.TypeResolver.ResolveType(className);

            try {
                var generator = (IAdaptiveFeatureGenerator) Activator.CreateInstance(type);
                var customGenerator = generator as CustomFeatureGenerator;

                if (customGenerator == null) 
                    return generator;

                var properties = generatorElement.Attributes.Cast<XmlAttribute>()
                    .Where(attribute => attribute.Name != "class")
                    .ToDictionary(attribute => attribute.Name, attribute => attribute.Value);

                if (provider != null) {
                    customGenerator.Init(properties, provider);
                }

                return generator;
            } catch (Exception ex) {
                throw new InvalidOperationException("Unable to create the feature generator.", ex);               
            }
        }
    }
}