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

using System;
using System.Collections.Generic;
using System.IO;
using SharpNL.Chunker;
using SharpNL.ML.Model;
using SharpNL.Parser.Lang;
using SharpNL.POSTag;
using SharpNL.Utility;
using SharpNL.Utility.Model;
using SharpNL.Utility.Serialization;

namespace SharpNL.Parser {
    /// <summary>
    /// This is an abstract base class for <see cref="ParserModel"/> implementations.
    /// </summary>
    public class ParserModel : BaseModel {
        private const string ComponentName = "Parser";
        private const string EntryBuildModel = "build.model";
        private const string EntryCheckModel = "check.model";
        private const string EntryAttachModel = "attach.model";
        private const string EntryParserTaggerModel = "parsertager.postagger";
        private const string EntryChunkerTaggerModel = "parserchunker.chunker";
        private const string EntryHeadRules = "head-rules.headrules";

        /// <summary>
        /// The parser type parameter in the manifest.
        /// </summary>
        private const string ParserTypeParameter = "parser-type";

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserModel"/> using the specified models and head rules.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="buildModel">The model to assign constituent labels.</param>
        /// <param name="checkModel">The model to determine a constituent is complete.</param>
        /// <param name="attachModel">The attach model.</param>
        /// <param name="parserTagger">The model to assign pos-tags.</param>
        /// <param name="chunkerTagger">The model to assign flat constituent labels.</param>
        /// <param name="headRules">The head rules.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <exception cref="System.ArgumentException">
        /// If the <paramref name="modelType"/> is equal to <see cref="Parser.ParserType.Chunking"/> the <paramref name="attachModel"/> must be <c>null</c>.
        /// or
        /// If the <paramref name="modelType"/> is equal to <see cref="Parser.ParserType.TreeInsert"/> the <paramref name="attachModel"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Unknown <paramref name="modelType"/> value.
        /// </exception>
        public ParserModel(
            string languageCode,
            IMaxentModel buildModel,
            IMaxentModel checkModel,
            IMaxentModel attachModel,
            POSModel parserTagger,
            ChunkerModel chunkerTagger,
            AbstractHeadRules headRules,
            ParserType modelType,
            Dictionary<string, string> manifestInfoEntries) : base(ComponentName, languageCode, manifestInfoEntries) {

            switch (modelType) {
                case ParserType.Chunking:
                    if (attachModel != null)
                        throw new ArgumentException(@"attachModel must be null for chunking parser!", nameof(attachModel));

                    Manifest[ParserTypeParameter] = "CHUNKING";
                    break;
                case ParserType.TreeInsert:
                    if (attachModel == null)
                        throw new ArgumentException(@"attachModel must not be null for treeinsert parser!",
                            nameof(attachModel));

                    Manifest[ParserTypeParameter] = "TREEINSERT";

                    artifactMap[EntryAttachModel] = attachModel;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modelType), "Unknown model type");
            }

            artifactMap[EntryBuildModel] = buildModel;
            artifactMap[EntryCheckModel] = checkModel;
            artifactMap[EntryParserTaggerModel] = parserTagger;
            artifactMap[EntryChunkerTaggerModel] = chunkerTagger;
            artifactMap[EntryHeadRules] = headRules;

            CheckArtifactMap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserModel"/> using the specified models and head rules without manifest information entries.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="buildModel">The model to assign constituent labels.</param>
        /// <param name="checkModel">The model to determine a constituent is complete.</param>
        /// <param name="attachModel">The attach model.</param>
        /// <param name="parserTagger">The model to assign pos-tags.</param>
        /// <param name="chunkerTagger">The model to assign flat constituent labels.</param>
        /// <param name="headRules">The head rules.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <exception cref="System.ArgumentException">
        /// If the <paramref name="modelType"/> is equal to <see cref="Parser.ParserType.Chunking"/> the <paramref name="attachModel"/> must be <c>null</c>.
        /// or
        /// If the <paramref name="modelType"/> is equal to <see cref="Parser.ParserType.TreeInsert"/> the <paramref name="attachModel"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Unknown <paramref name="modelType"/> value.
        /// </exception>
        public ParserModel(
            string languageCode,
            IMaxentModel buildModel,
            IMaxentModel checkModel,
            IMaxentModel attachModel,
            POSModel parserTagger,
            ChunkerModel chunkerTagger,
            AbstractHeadRules headRules,
            ParserType modelType) : this(

                languageCode,
                buildModel,
                checkModel,
                attachModel,
                parserTagger,
                chunkerTagger,
                headRules,
                modelType,
                null) {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ParserModel"/> using the specified models and head rules using the model type as chunking.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="buildModel">The model to assign constituent labels.</param>
        /// <param name="checkModel">The model to determine a constituent is complete.</param>
        /// <param name="parserTagger">The model to assign pos-tags.</param>
        /// <param name="chunkerTagger">The model to assign flat constituent labels.</param>
        /// <param name="headRules">The head rules.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        public ParserModel(
            string languageCode,
            IMaxentModel buildModel,
            IMaxentModel checkModel,
            POSModel parserTagger,
            ChunkerModel chunkerTagger,
            AbstractHeadRules headRules,
            Dictionary<string, string> manifestInfoEntries) : this(

                languageCode,
                buildModel,
                checkModel,
                null,
                parserTagger,
                chunkerTagger,
                headRules,
                ParserType.Chunking,
                manifestInfoEntries) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserModel"/> class deserializing the input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="inputStream"/> is null.
        /// </exception>
        public ParserModel(Stream inputStream) : base(ComponentName, inputStream) {
            
        }

        #endregion

        #region + Properties .

        #region . AttachModel .

        public IMaxentModel AttachModel {
            get {
                if (artifactMap.ContainsKey(EntryAttachModel))
                    return artifactMap[EntryAttachModel] as IMaxentModel;

                return null;
            }
        }
        #endregion

        #region . BuildModel .

        public IMaxentModel BuildModel {
            get {
                if (artifactMap.ContainsKey(EntryBuildModel))
                    return artifactMap[EntryBuildModel] as IMaxentModel ;

                return null;
            }
        }
        #endregion

        #region . CheckModel .

        public IMaxentModel CheckModel {
            get {
                if (artifactMap.ContainsKey(EntryCheckModel))
                    return artifactMap[EntryCheckModel] as IMaxentModel;

                return null;
            }
        }
        #endregion

        #region . DefaultFactory .
        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type DefaultFactory => null;

        #endregion

        #region . HeadRules .

        public AbstractHeadRules HeadRules {
            get {
                if (artifactMap.ContainsKey(EntryHeadRules))
                    return artifactMap[EntryHeadRules] as AbstractHeadRules;

                return null;
            }
        }
        #endregion

        #region . ParserChunkerModel .

        public ChunkerModel ParserChunkerModel {
            get {
                if (artifactMap.ContainsKey(EntryChunkerTaggerModel))
                    return artifactMap[EntryChunkerTaggerModel] as ChunkerModel;

                return null;
            }
        }
        #endregion

        #region . ParserTaggerModel .

        public POSModel ParserTaggerModel {
            get {
                if (artifactMap.ContainsKey(EntryParserTaggerModel))
                    return artifactMap[EntryParserTaggerModel] as POSModel;

                return null;
            }
        }
        #endregion

        #region . ParserType .
        public ParserType ParserType {
            get {
                switch (Manifest[ParserTypeParameter]) {
                    case "CHUNKING":
                        return ParserType.Chunking;
                    case "TREEINSERT":
                        return ParserType.TreeInsert;
                }
                return default(ParserType);
            }
        }
        #endregion

        #endregion

        #region . CreateArtifactSerializers .

        /// <summary>
        /// Registers all serializers for their artifact file name extensions. Override this method to register custom file extensions.
        /// </summary>
        /// <seealso href="https://msdn.microsoft.com/en-us/library/ms182331.aspx" />
        /// <remarks>The subclasses should invoke the <see cref="ArtifactProvider.RegisterArtifactType" /> to register
        /// the proper serialization/deserialization methods for an new extension.
        /// Warning: This method is called in constructor of the base class!! Be aware that this method is ONLY designed to register serializers.</remarks>
        protected override void CreateArtifactSerializers() {
            base.CreateArtifactSerializers();
            // note from OpenNLP (for future adaptations)

            // In 1.6.x the headrules artifact is serialized with the new API
            // which uses the Serializable interface
            // This change is not backward compatible with the 1.5.x models.
            // In order to load 1.5.x model the English headrules serializer must be
            // put on the serializer map.

            RegisterArtifactType(".headrules",
                (artifact, stream) => HeadRulesManager.Serialize(artifact as AbstractHeadRules, stream),
                stream => HeadRulesManager.Deserialize(Language, stream));

            RegisterArtifactType(".postagger", (artifact, stream) => {
                var model = artifact as POSModel;
                if (model == null)
                    throw new InvalidOperationException();

                model.Serialize(stream);
            }, stream => {
                var model = new POSModel(stream);

                // The 1.6.x models write the non-default beam size into the model itself.
                // In 1.5.x the parser configured the beam size when the model was loaded,
                // this is not possible anymore with the new APIs
                if (model.Version.Major == 1 && model.Version.Minor == 5 && !model.Manifest.Contains(Parameters.BeamSize))
                    return new POSModel(model.Language, model.MaxentModel, 10, null, model.Factory);

                return model;
            });

            RegisterArtifactType(".chunker", (artifact, stream) => {
                var model = artifact as ChunkerModel;
                if (model == null)
                    throw new InvalidOperationException();

                model.Serialize(stream);
            }, stream => {
                var model = new ChunkerModel(stream);

                if (model.Version.Major == 1 && model.Version.Minor == 5) {
                    return new ChunkerModel(model.Language, model.MaxentModel, new ParserChunkerFactory());
                }

                return model;
            });
        }

        #endregion

        #region . UpdateBuildModel .
        public ParserModel UpdateBuildModel(IMaxentModel buildModel) {
            return new ParserModel(Language, buildModel, CheckModel, AttachModel, ParserTaggerModel, ParserChunkerModel, HeadRules, ParserType);
        }
        #endregion

        #region . UpdateCheckModel .
        public ParserModel UpdateCheckModel(IMaxentModel checkModel) {
            return new ParserModel(Language, BuildModel, checkModel, AttachModel, ParserTaggerModel, ParserChunkerModel, HeadRules, ParserType);
        }
        #endregion

        #region . UpdateTaggerModel .
        public ParserModel UpdateTaggerModel(POSModel taggerModel) {
            return new ParserModel(Language, BuildModel, CheckModel, AttachModel, taggerModel, ParserChunkerModel, HeadRules, ParserType);
        }
        #endregion

        #region . UpdateChunkerModel .
        public ParserModel UpdateChunkerModel(ChunkerModel chunkModel) {
            return new ParserModel(Language, BuildModel, CheckModel, AttachModel, ParserTaggerModel, chunkModel, HeadRules, ParserType);
        }
        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke super.validateArtifactMap at the beginning of this method.</remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            if (artifactMap.ContainsKey(EntryBuildModel) &&
                (artifactMap[EntryBuildModel] as AbstractModel) == null) {
                throw new InvalidFormatException("Missing the build model!");
            }

            if (ParserType == ParserType.Chunking) {
                if (AttachModel != null)
                    throw new InvalidFormatException("attachModel must be null for chunking parser!");
            } else if (ParserType == ParserType.TreeInsert) {
                if (AttachModel == null)
                    throw new InvalidFormatException("attachModel must not be null!");
            } else {
                throw new InvalidFormatException("Unknown parser type.");
            }

            if (CheckModel == null)
                throw new InvalidFormatException("Missing the check model!");

            if (ParserChunkerModel == null)
                throw new InvalidFormatException("Missing the chunker model!");

            if (ParserTaggerModel == null)
                throw new InvalidFormatException("Missing the tagger model!");

            if (HeadRules == null)
                throw new InvalidFormatException("Missing the head rules!");

        }
        #endregion

    }
}