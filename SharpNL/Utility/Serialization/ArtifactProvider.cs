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
using System.IO;
using System.Collections.Generic;

#if ZIPLIB
using ICSharpCode.SharpZipLib.Zip;
using SharpNL.ML.Model;
#else
using System.IO.Compression;
#endif

namespace SharpNL.Utility.Serialization {

    /// <summary>
    /// Provides access to model persisted artifacts.
    /// </summary>
    public abstract class ArtifactProvider {

        internal const string FactoryName = "factory";
        internal const string ManifestEntry = "manifest.properties";
        internal const string ManifestVersionProperty = "Manifest-Version";

        internal const string LanguageEntry = "Language";
        internal const string VersionProperty = "OpenNLP-Version";
        
        internal const string ComponentNameEntry = "Component-Name";
        internal const string TimestampEntry = "Timestamp";

        internal const string TrainingCutoffProperty = "Training-Cutoff";
        internal const string TrainingIterationsProperty = "Training-Iterations";
        internal const string TrainingEventhashProperty = "Training-Eventhash";

        private readonly Dictionary<string, Serializer> artifactSerializers;

        /// <summary>
        /// The artifact map
        /// </summary>
        protected readonly Dictionary<string, object> artifactMap;

        private class Serializer {
            public SerializeDelegate Serialize;
            public DeserializeDelegate Deserialize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtifactProvider"/> class.
        /// </summary>
        protected ArtifactProvider() {
            artifactSerializers = new Dictionary<string, Serializer>();

            artifactMap = new Dictionary<string, object> {
                {ManifestEntry, new Properties()}
            };
        }

        #region + Properties .

        #region . FinishedLoadingArtifacts .
        /// <summary>
        /// Gets a value indicating whether artifacts are completely loaded.
        /// </summary>
        /// <value><c>true</c> if the artifacts are completely loaded; otherwise, <c>false</c>.</value>
        protected bool FinishedLoadingArtifacts { get; private set; }
        #endregion

        #region . Manifest .
        /// <summary>
        /// Gets the manifest property collection.
        /// </summary>
        /// <value>The manifest property collection.</value>
        public Properties Manifest {
            get {
                if (artifactMap.ContainsKey(ManifestEntry)) {
                    return artifactMap[ManifestEntry] as Properties;
                }
                return null;
            }
        }
        #endregion

        #region . IsLoadedFromSerialized .
        /// <summary>
        /// Gets a value indicating if this provider was loaded from serialized.
        /// </summary>
        /// <value><c>true</c> if this model was loaded from serialized; otherwise, <c>false</c>.</value>
        /// <remarks>It is useful, for example, while validating artifacts: you can skip the time consuming ones if they where already validated during the serialization. 
        /// </remarks>
        public bool IsLoadedFromSerialized { get; protected set; }
        #endregion

        #endregion

        #region . Contains .

        /// <summary>
        /// Determines whether this artifact provider contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this artifact provider contains the specified key; otherwise, <c>false</c>.</returns>
        public bool Contains(string key) {
            return artifactMap.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether this artifact provider contains the specified key and the artifact is instance of the given type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this artifact provider contains the specified key; otherwise, <c>false</c>.</returns>
        public bool Contains(string key, Type type) {
            return artifactMap.ContainsKey(key) && type.IsInstanceOfType(artifactMap[key]);
        }

        #endregion

        #region . GetArtifact .
        /// <summary>
        /// Gets an artifact with the specified key.
        /// </summary>
        /// <typeparam name="T">The artifact type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The artifact.</returns>
        public T GetArtifact<T>(string key) {
            // ReSharper disable once UseIsOperator.2
            if (artifactMap.ContainsKey(key) && typeof(T).IsInstanceOfType(artifactMap[key])) {
                return (T)artifactMap[key];
            }
            return default(T);
        }
        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke base.ValidateArtifactMap at the beginning of this method.</remarks>
        protected virtual void ValidateArtifactMap() {
            if (!artifactMap.ContainsKey(ManifestEntry) || !(artifactMap[ManifestEntry] is Properties)) {
                throw new InvalidFormatException("Unable to find the manifest entry.");
            }
        }
        #endregion

        #region . ManifestDeserialized .
        protected virtual void ManifestDeserialized() {
            // nothing to do here
        }
        #endregion

        #region . Deserialize .

        /// <summary>
        /// Deserializes the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="InvalidFormatException">Unable to find the manifest file.</exception>
        protected void Deserialize(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException(nameof(inputStream));
            }

            var isSearchingForManifest = true;

            try {

                #if ZIPLIB
                var lazyStack = new Stack<LazyArtifact>();
                using (var zip = new ZipInputStream(new UnclosableStream(inputStream))) {
                    ZipEntry entry;
                    while ((entry = zip.GetNextEntry()) != null) {
                        if (entry.Name == ManifestEntry) {
                            isSearchingForManifest = false;

                            artifactMap[ManifestEntry] = Properties.Deserialize(new UnclosableStream(zip));

                            zip.CloseEntry();

                            ManifestDeserialized();

                            CreateArtifactSerializers();

                            FinishLoadingArtifacts(lazyStack, zip);

                            break;
                        }

                        lazyStack.Push(new LazyArtifact(entry, zip));

                        zip.CloseEntry();
                    }
                }
                #else
                using (var zip = new ZipArchive(inputStream, ZipArchiveMode.Read, true)) {
                    
                    foreach (var entry in zip.Entries) {
                        if (entry.Name != ManifestEntry)
                            continue;

                        isSearchingForManifest = false;

                        using (var stream = entry.Open())
                            artifactMap[ManifestEntry] = Properties.Deserialize(stream);

                        ManifestDeserialized();

                        CreateArtifactSerializers();

                        FinishLoadingArtifacts(zip);


                        break;
                    }
                }
                #endif
            } catch (Exception ex) {
                throw new InvalidOperationException("An error had occurred during the deserialization of the model file.", ex);
            }

            if (!isSearchingForManifest) 
                return;

            throw new InvalidFormatException("Unable to find the manifest file.");
        }

        #endregion

        #region . RegisterArtifactType .
        /// <summary>
        /// Registers an artifact type with his serialization methods.
        /// </summary>
        /// <param name="name">The artifact extension.</param>
        /// <param name="serialize">The serialization method.</param>
        /// <param name="deserialize">The deserialization method.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/>
        /// or
        /// <paramref name="serialize"/>
        /// or
        /// <paramref name="deserialize"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The specified artifact name is already registered.</exception>
        public void RegisterArtifactType(string name, SerializeDelegate serialize, DeserializeDelegate deserialize) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }
            if (serialize == null) {
                throw new ArgumentNullException(nameof(serialize));
            }
            if (deserialize == null) {
                throw new ArgumentNullException(nameof(deserialize));
            }

            if (!name.StartsWith(".")) {
                name = "." + name;
            }

            if (artifactSerializers.ContainsKey(name)) {
                throw new ArgumentException(@"The specified artifact name is already registered.", nameof(name));
            }

            artifactSerializers.Add(name, new Serializer {
                Serialize = serialize,
                Deserialize = deserialize
            });
        }
        #endregion

        #region . CreateArtifactSerializers .
        /// <summary>
        /// Registers all serializers for their artifact file name extensions. Override this method to register custom file extensions.
        /// </summary>
        /// <remarks>
        /// The subclasses should invoke the <see cref="ArtifactProvider.RegisterArtifactType"/> to register 
        /// the proper serialization/deserialization methods for an new extension.
        /// Warning: This method is called in constructor of the base class!! Be aware that this method is ONLY designed to register serializers.
        /// </remarks>
        /// <seealso href="https://msdn.microsoft.com/en-us/library/ms182331.aspx"/>
        protected virtual void CreateArtifactSerializers() {
            RegisterArtifactType(".properties", Properties.Serialize, Properties.Deserialize);
            RegisterArtifactType(".dictionary", Dictionary.Dictionary.Serialize, Dictionary.Dictionary.Deserialize);          
        }

        #endregion

        #region . FinishLoadingArtifacts .

        /// <summary>
        /// Finish loading the artifacts now that it knows all serializers.
        /// </summary>
        #if ZIPLIB
        private void FinishLoadingArtifacts(Stack<LazyArtifact> lazyStack, ZipInputStream zip) {
            // process the lazy artifacts
            while (lazyStack.Count > 0) {
                using (var lazy = lazyStack.Pop()) {
                    LoadArtifact(lazy.Name, lazy.Data);
                }
            }

            // process the "normal" artifacts
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null) {
                if (entry.Name != ManifestEntry) {
                    LoadArtifact(entry.Name, zip);
                }
                zip.CloseEntry();
            }

            FinishedLoadingArtifacts = true;
        }
        #else
        private void FinishLoadingArtifacts(ZipArchive zip) {
            foreach (var entry in zip.Entries)
            {
                if (entry.Name == ManifestEntry)
                    continue;

                using (var stream = entry.Open())
                //using (var reader = new StreamReader(stream))
                    LoadArtifact(entry.Name, stream);

            }
            FinishedLoadingArtifacts = true;
        }
        #endif

        #endregion

        #region . LoadArtifact .

        /// <summary>
        /// Loads the artifact with the given name.
        /// </summary>
        /// <param name="name">The artifact name.</param>
        /// <param name="dataStream">The data stream.</param>
        /// <exception cref="InvalidFormatException">Unknown artifact format.</exception>
        /// <exception cref="System.InvalidOperationException">The deserializer is not registered.</exception>
        private void LoadArtifact(string name, Stream dataStream) {
            var ext = Path.GetExtension(name);
            if (string.IsNullOrEmpty(ext) || !artifactSerializers.ContainsKey(ext))
                throw new InvalidFormatException("Unknown artifact format: " + name);

            if (artifactSerializers[ext] == null)
                throw new InvalidOperationException("The deserializer is not registered.");

            artifactMap[name] = artifactSerializers[ext].Deserialize(dataStream);
        }

        #endregion

        #region + Serialize .
        /// <summary>
        /// Serializes the model to the given <see cref="T:Stream" />
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="outputStream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">The specified <paramref name="outputStream"/> is not writable.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// Invalid artifact entry name.
        /// or
        /// Missing serializer for the artifact.
        /// </exception>
        public void Serialize(Stream outputStream) {

            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));

            if (!outputStream.CanWrite)
                throw new ArgumentException(@"The specified stream is not writable.", nameof(outputStream));

            #if ZIPLIB

            using(var zip = new ZipOutputStream(outputStream)) {

                foreach (var artifact in artifactMap) {

                    var ext = Path.GetExtension(artifact.Key);
                    if (string.IsNullOrEmpty(ext))
                        throw new InvalidOperationException("Invalid artifact entry name.");

                    if (!artifactSerializers.ContainsKey(ext))
                        throw new InvalidOperationException("Missing serializer for " + artifact.Key);

                    zip.PutNextEntry(new ZipEntry(artifact.Key));

                    artifactSerializers[ext].Serialize(artifact.Value, new UnclosableStream(zip));

                    zip.CloseEntry();
                }
                
                zip.Flush();
                zip.Close();
            }
            #else
            using (var zip = new ZipArchive(outputStream, ZipArchiveMode.Create)) {
                foreach (var artifact in artifactMap) {

                    var ext = Path.GetExtension(artifact.Key);
                    if (string.IsNullOrEmpty(ext))
                        throw new InvalidOperationException("Invalid artifact entry name.");

                    if (!artifactSerializers.ContainsKey(ext))
                        throw new InvalidOperationException("Missing serializer for " + artifact.Key);

                    var entry = zip.CreateEntry(artifact.Key);
                    var serialization = artifactSerializers[ext];

                    using (var stream = entry.Open())
                        serialization.Serialize(artifact.Value, stream);

                }
            }
            #endif
        }

        /// <summary>
        /// Serializes the model to the given filename. If the specified file already exist, the serializer will overwrite the existing file.
        /// </summary>
        /// <param name="fileName">A relative or absolute path for the file that the current object will be serialized.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="fileName"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="fileName"/> is an empty string (""), contains only white space, or contains one
        /// or more invalid characters. -or-path refers to a non-file device, such as
        /// "con:", "com1:", "lpt1:", etc. in an NTFS environment.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum 
        /// length. For example, on Windows-based platforms, paths must be less than
        /// 248 characters, and file names must be less than 260 characters.
        /// </exception>
        public void Serialize(string fileName) {
            if (string.IsNullOrEmpty(fileName)) 
                throw new ArgumentNullException(nameof(fileName));

            using (var stream = new FileStream(fileName, FileMode.Create))
                Serialize(stream);

        }

        #endregion
    }
}