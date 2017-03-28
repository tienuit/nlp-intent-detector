//  
//  Copyright 2016 Gustavo J Knuppe (https://github.com/knuppe)
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
using System.Globalization;
using System.IO;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.Model;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents a model used by a learnable <see cref="ILemmatizer" />.
    /// </summary>
    public class LemmatizerModel : BaseModel {
        private const string ComponentName = "StatisticalLemmatizer";
        private const string ModelEntry = "lemmatizer.model";


        public LemmatizerModel(string languageCode, ISequenceClassificationModel<string> lemmatizerModel, Dictionary<string, string> manifestInfoEntries, LemmatizerFactory factory)
            : base(ComponentName, languageCode, manifestInfoEntries, factory) {
            artifactMap.Add(ModelEntry, lemmatizerModel);
            CheckArtifactMap();
        }

        public LemmatizerModel(string languageCode, IMaxentModel lemmatizerModel, Dictionary<string, string> manifestInfoEntries, LemmatizerFactory factory)
            : this(languageCode, lemmatizerModel, LemmatizerME.DefaultBeamSize, manifestInfoEntries, factory) {
        }

        public LemmatizerModel(string languageCode, IMaxentModel lemmatizerModel, int beamSize, Dictionary<string, string> manifestInfoEntries, LemmatizerFactory factory)
            : base(ComponentName, languageCode, manifestInfoEntries, factory) {
            artifactMap.Add(ModelEntry, lemmatizerModel);

            Manifest[Parameters.BeamSize] = beamSize.ToString(CultureInfo.InvariantCulture);

            CheckArtifactMap();
        }

        public LemmatizerModel(string languageCode, IMaxentModel lemmatizerModel, LemmatizerFactory factory)
            : this(languageCode, lemmatizerModel, null, factory) {
        }

        public LemmatizerModel(Stream stream) : base(ComponentName, stream) {
        }

        public LemmatizerModel(string fileName) : base(ComponentName, fileName) {
        }

        /// <summary>
        /// Gets the lemmatizer sequence model.
        /// </summary>
        /// <value>The lemmatizer sequence model.</value>
        public ISequenceClassificationModel<string> LemmatizerSequenceModel {
            get {
                var model = artifactMap[ModelEntry] as IMaxentModel;

                if (model == null)
                    return artifactMap[ModelEntry] as ISequenceClassificationModel<string>;

                var beamSize = Manifest.Get(Parameters.BeamSize, LemmatizerME.DefaultBeamSize);

                return new ML.BeamSearch<string>(beamSize, model);
            }
        }

        /// <summary>
        /// Gets the lemmatizer tool factory.
        /// </summary>
        public LemmatizerFactory Factory => (LemmatizerFactory) ToolFactory;

        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <value>The default factory.</value>
        protected override Type DefaultFactory => typeof (LemmatizerFactory);

        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Lemmatizer model is incomplete!</exception>
        /// <remarks>Subclasses should generally invoke base. ValidateArtifactMap at the beginning of this method.</remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            if (!artifactMap.ContainsKey(ModelEntry) || !(artifactMap[ModelEntry] is AbstractModel))
                throw new InvalidFormatException("Lemmatizer model is incomplete!");
        }
    }
}