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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SharpNL.Extensions;
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Formats {
    /// <summary>
    /// Parser for the dutch and spanish ner training files of the CONLL 2002 shared task.
    /// <para>
    /// The dutch data has a -DOCSTART- tag to mark article boundaries,
    /// adaptive data in the feature generators will be cleared before every article.<br />
    /// The spanish data does not contain article boundaries,
    /// adaptive data will be cleared for every sentence.
    /// </para>
    /// <para>The data contains four named entity types: Person, Organization, Location and Misc.</para>
    /// </summary>
    /// <remarks>
    /// Data can be found on this web site: <see href="http://www.cnts.ua.ac.be/conll2002/ner/" />
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public class CoNLL02NameSampleStream : CoNLL, IObjectStream<NameSample> {

        internal const string DocStart = "-DOCSTART-";

        internal readonly Language language;
        internal readonly IObjectStream<string> lineStream;

        private readonly bool ownsStream;

        internal readonly Types types;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL02NameSampleStream"/> class.
        /// </summary>
        /// <param name="language">The supported conll language.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="types">The conll types.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="language"/></exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="lineStream"/></exception>
        /// <exception cref="System.ArgumentException">The specified language is not supported.</exception>
        public CoNLL02NameSampleStream(Language language, IObjectStream<string> lineStream, Types types) : this(language, lineStream, types, true) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL02NameSampleStream" /> class.
        /// </summary>
        /// <param name="language">The supported conll language.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="types">The conll types.</param>
        /// <param name="ownsStream"><c>true</c> to indicate that the stream will be disposed when this stream is disposed; <c>false</c> to indicate that the stream will not be disposed when this stream is disposed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="language" /></exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="lineStream" /></exception>
        /// <exception cref="System.ArgumentException">The specified language is not supported.</exception>
        public CoNLL02NameSampleStream(Language language, IObjectStream<string> lineStream, Types types, bool ownsStream) {
            if (!Enum.IsDefined(typeof(Language), language))
                throw new ArgumentOutOfRangeException(nameof(language));

            if (lineStream == null)
                throw new ArgumentNullException(nameof(lineStream));

            if (!language.In(Language.En, Language.De))
                throw new ArgumentException("The specified language is not supported.");

            this.language = language;
            this.lineStream = lineStream;
            this.ownsStream = ownsStream;
            this.types = types;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL02NameSampleStream"/> class.
        /// </summary>
        /// <param name="language">The supported conll language.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="types">The conll types.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="language"/>
        /// or
        /// <paramref name="types"/>
        /// </exception>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        public CoNLL02NameSampleStream(Language language, Stream inputStream, Types types) {
            if (!Enum.IsDefined(typeof(Language), language))
                throw new ArgumentOutOfRangeException(nameof(language));

            if (!Enum.IsDefined(typeof(Types), types))
                throw new ArgumentOutOfRangeException(nameof(types));

            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            this.language = language;
            lineStream = new PlainTextByLineStream(inputStream);
            this.types = types;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL02NameSampleStream"/> class.
        /// </summary>
        /// <param name="language">The supported conll language.</param>
        /// <param name="streamFactory">The stream factory.</param>
        /// <param name="types">The conll types.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">language</exception>
        /// <exception cref="System.ArgumentNullException">streamFactory</exception>
        public CoNLL02NameSampleStream(Language language, IInputStreamFactory streamFactory, Types types) {
            if (!Enum.IsDefined(typeof(Language), language))
                throw new ArgumentOutOfRangeException(nameof(language));

            if (streamFactory == null)
                throw new ArgumentNullException(nameof(streamFactory));

            this.language = language;
            lineStream = new PlainTextByLineStream(streamFactory);
            this.types = types;
        }

        #endregion

        #region . Dispose .

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            if (ownsStream && lineStream != null)
                lineStream.Dispose();
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
        public NameSample Read() {
            var sentence = new List<string>();
            var tags = new List<string>();

            var ClearAdaptiveData = false;

            // Empty line indicates end of sentence
            string line;
            while ((line = lineStream.Read()) != null && !string.IsNullOrWhiteSpace(line)) {
                if (language == Language.Nl && line.StartsWith(DocStart)) {
                    ClearAdaptiveData = true;
                    continue;
                }

                var fields = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (fields.Length == 3) {
                    sentence.Add(fields[0]);
                    tags.Add(fields[2]);
                } else {
                    throw new InvalidFormatException(
                        $"Expected three fields per line in training data, got {fields.Length} for line '{line}'!");
                }
            }

            // Always clear adaptive data for spanish
            if (language == Language.Es)
                ClearAdaptiveData = true;

            if (sentence.Count > 0) {
                // convert name tags into spans
                var names = new List<Span>();

                var beginIndex = -1;
                var endIndex = -1;
                for (var i = 0; i < tags.Count; i++) {
                    var tag = tags[i];

                    if (tag.EndsWith("PER") && (types & Types.PersonEntities) == 0)
                        tag = "O";

                    if (tag.EndsWith("ORG") && (types & Types.OrganizationEntities) == 0)
                        tag = "O";

                    if (tag.EndsWith("LOC") && (types & Types.LocationEntities) == 0)
                        tag = "O";

                    if (tag.EndsWith("MISC") && (types & Types.MiscEntities) == 0)
                        tag = "O";

                    if (tag.StartsWith("B-")) {
                        if (beginIndex != -1) {
                            names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                            //beginIndex = -1;
                            //endIndex = -1;
                        }

                        beginIndex = i;
                        endIndex = i + 1;
                    } else if (tag.StartsWith("I-")) {
                        endIndex++;
                    } else if (tag.Equals("O")) {
                        if (beginIndex != -1) {
                            names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                            beginIndex = -1;
                            endIndex = -1;
                        }
                    } else {
                        throw new InvalidFormatException("Invalid tag: " + tag);
                    }
                }

                // if one span remains, create it here
                if (beginIndex != -1)
                    names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));

                return new NameSample(sentence.ToArray(), names.ToArray(), ClearAdaptiveData);
            }

            return line != null ? Read() : null;
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            lineStream.Reset();
        }

        #endregion

    }
}