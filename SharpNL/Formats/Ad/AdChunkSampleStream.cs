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
using System.Text;
using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Represents a <see cref="ChunkSample"/> sample stream for floresta Sita(c)tica Arvores Deitadas corpus.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public class AdChunkSampleStream : Disposable, IObjectStream<ChunkSample> {
        private const string Other = "O";

        /// <summary>
        /// The Ad sentence stream.
        /// </summary>
        protected readonly IObjectStream<AdSentence> adSentenceStream;

        /// <summary>
        /// The evaluation monitor.
        /// </summary>
        protected readonly Monitor monitor;

        #region + Constructors .

        /// <summary>
        /// Prevents a default instance of the <see cref="AdChunkSampleStream"/> class from being created.
        /// </summary>
        private AdChunkSampleStream() {
            Index = 0;
            Start = -1;
            End = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdChunkSampleStream"/> class.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid AD sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="lineStream"/>
        /// </exception>
        public AdChunkSampleStream(IObjectStream<string> lineStream, bool safeParse) : this() {
            if (lineStream == null)
                throw new ArgumentNullException(nameof(lineStream));

            adSentenceStream = new AdSentenceStream(lineStream, safeParse);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdChunkSampleStream"/> class.
        /// </summary>
        /// <param name="monitor">The execution monitor.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid AD sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="monitor"/>
        /// or
        /// <paramref name="lineStream"/>
        /// </exception>
        public AdChunkSampleStream(Monitor monitor, IObjectStream<string> lineStream, bool safeParse)
            : this(lineStream, safeParse) {
            if (monitor == null)
                throw new ArgumentNullException(nameof(monitor));

            this.monitor = monitor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdChunkSampleStream" /> from a <paramref name="inputStream" /> object.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The text encoding used to read the stream.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/>
        /// or
        /// <paramref name="encoding"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The <paramref name="inputStream" /> does not support reading.</exception>
        public AdChunkSampleStream(Stream inputStream, Encoding encoding, bool safeParse)
            : this() {

            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (!inputStream.CanRead)
                throw new ArgumentException("The input stream does not support reading.");

            adSentenceStream = new AdSentenceStream(new PlainTextByLineStream(inputStream, encoding), safeParse);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdChunkSampleStream" /> from a <paramref name="inputStream" /> object.
        /// </summary>
        /// <param name="monitor">The execution monitor.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The text encoding used to read the stream.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/>
        /// or
        /// <paramref name="encoding"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The <paramref name="inputStream" /> does not support reading.</exception>
        public AdChunkSampleStream(Monitor monitor, Stream inputStream, Encoding encoding, bool safeParse)
            : this(inputStream, encoding, safeParse) {
            if (monitor == null)
                throw new ArgumentNullException(nameof(monitor));

            this.monitor = monitor;
        }

        #endregion

        #region + Properties .

        #region . End .
        /// <summary>
        /// Gets or sets the end position. The default value is -1;
        /// </summary>
        /// <value>The end position.</value>
        public int End { get; set; }
        #endregion

        #region . Index .
        /// <summary>
        /// Gets the current index.
        /// </summary>
        /// <value>The current index.</value>
        public int Index { get; private set; }
        #endregion

        #region . IncludePunctuations .
        /// <summary>
        /// Gets a value indicating whether punctuations should be included. The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if punctuations should be included; otherwise, <c>false</c>.</value>
        public bool IncludePunctuations { get; protected set; }
        #endregion

        #region . Start .
        /// <summary>
        /// Gets or sets the start position. The default value is -1;
        /// </summary>
        /// <value>The start position.</value>
        public int Start { get; set; }
        #endregion

        #endregion

        #region . DisposeManagedResources .

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            adSentenceStream.Dispose();
        }

        #endregion

        #region . GetChunkTag .

        /// <summary>
        /// Gets the chunk tag.
        /// </summary>
        /// <param name="leaf">The leaf.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetChunkTag(AdLeaf leaf) {
            return leaf.SecondaryTag == "P" ? "VP" : null;
        }

        /// <summary>
        /// Gets the chunk tag.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetChunkTag(AdNode node, string parent, int index) {
            var tag = node.SyntacticTag;

            var phraseTag = tag.Substring(tag.LastIndexOf(":", StringComparison.Ordinal) + 1);

            while (phraseTag.EndsWith("-")) {
                phraseTag = phraseTag.Substring(0, phraseTag.Length - 1);
            }

            if (phraseTag == "adjp" && parent != "NP") {
                phraseTag = "np";
            }

            // maybe we should use only np, vp and pp, but will keep ap and advp.
            if (phraseTag.Equals("np") ||
                phraseTag.Equals("vp") ||
                phraseTag.Equals("pp") ||
                phraseTag.Equals("ap") ||
                phraseTag.Equals("advp") 
                // || phraseTag.equals("adjp")
                // || phraseTag.equals("cu") 
                // || phraseTag.equals("sq")
                ) {
                phraseTag = phraseTag.ToUpperInvariant();
            } else {
                phraseTag = Other;
            }

            return phraseTag;
        }

        #endregion

        #region . GetPhraseTagFromPosTag .

        /// <summary>
        /// Gets the phrase tag from position tag.
        /// </summary>
        /// <param name="functionalTag">The functional tag.</param>
        /// <returns>System.String.</returns>
        protected static string GetPhraseTagFromPosTag(string functionalTag) {
            switch (functionalTag) {
                case "v-fin":
                    return "VP";
                case "n":
                    return "NP";
                default:
                    return Other;
            }
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
        public ChunkSample Read() {
            AdSentence paragraph;
            while ((paragraph = adSentenceStream.Read()) != null) {
                if (End > -1 && Index >= End) { // leave
                    return null;
                }

                if (Start > -1 && Index < Start) {
                    Index++;
                } else {
                    var root = paragraph.Root;
                    var sentence = new List<string>();
                    var tags = new List<string>();
                    var target = new List<string>();

                    ProcessRoot(root, sentence, tags, target);

                    if (sentence.Count <= 0) 
                        continue;

                    Index++;
                    return new ChunkSample(sentence.ToArray(), tags.ToArray(), target.ToArray());
                }
            }
            return null;
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            adSentenceStream.Reset();
        }

        #endregion

        #region . ProcessRoot .
        /// <summary>
        /// Processes the root node.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="sentence">The sentence.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="target">The target.</param>
        protected void ProcessRoot(AdNode root, List<string> sentence, List<string> tags, List<string> target) {
            if (root == null)
                return;

            foreach (var element in root.Elements) {
                if (element.IsLeaf) {
                    ProcessLeaf((AdLeaf)element, false, Other, sentence, tags, target);
                } else {
                    ProcessNode((AdNode)element, sentence, tags, target, null);
                }
            }
        }
        #endregion

        #region . ProcessNode .

        private void ProcessNode(AdNode node, List<string> sentence, List<string> tags,
            List<string> target, String inheritedTag) {
            var phraseTag = GetChunkTag(node, inheritedTag, target.Count);

            var inherited = false;
            if ((phraseTag == Other /*|| phraseTag.equals(inheritedTag)*/) && inheritedTag != null) {
                phraseTag = inheritedTag;
                inherited = true;
            }

            for (var i = 0; i < node.Elements.Count; i++) {
                if (node.Elements[i].IsLeaf) {
                    var isIntermediate = false;
                    var tag = phraseTag;
                    var leaf = (AdLeaf) node.Elements[i];

                    var localChunk = GetChunkTag(leaf);
                    if (localChunk != null && !tag.Equals(localChunk)) {
                        tag = localChunk;
                    }

                    if (IsIntermediate(tags, target, tag) && (inherited || i > 0)) {
                        isIntermediate = true;
                    }

                    if (!IncludePunctuations && leaf.FunctionalTag == null && (
                        !(i + 1 < node.Elements.Count && node.Elements[i + 1].IsLeaf) ||
                        !(i > 0 && node.Elements[i - 1].IsLeaf))) {
                        isIntermediate = false;
                        tag = Other;
                    }
                    ProcessLeaf(leaf, isIntermediate, tag, sentence, tags, target);
                } else {
                    var before = target.Count;

                    ProcessNode((AdNode) node.Elements[i], sentence, tags, target, phraseTag);

                    // if the child node was of a different type we should break the chunk sequence
                    for (var j = target.Count - 1; j >= before; j--) {
                        if (!target[j].EndsWith("-" + phraseTag)) {
                            phraseTag = Other;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region . IsIntermediate .
        /// <summary>
        /// Determines whether the specified tags is intermediate.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="target">The target.</param>
        /// <param name="phraseTag">The phrase tag.</param>
        /// <returns><c>true</c> if the specified tags is intermediate; otherwise, <c>false</c>.</returns>
        protected bool IsIntermediate(List<string> tags, List<string> target, string phraseTag) {
            return target.Count > 0 && target[target.Count - 1].EndsWith("-" + phraseTag);
        }
        #endregion

        #region . ProcessLeaf .

        /// <summary>
        /// Processes the Ad leaf.
        /// </summary>
        /// <param name="leaf">The leaf.</param>
        /// <param name="isIntermediate">if set to <c>true</c> leaf is intermediate.</param>
        /// <param name="phraseTag">The phrase tag.</param>
        /// <param name="sentence">The sentence.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="target">The target.</param>
        protected void ProcessLeaf(
            AdLeaf leaf,
            bool isIntermediate,
            string phraseTag,
            List<string> sentence,
            List<string> tags,
            List<string> target) {
            string chunkTag;

            if (leaf.FunctionalTag != null && phraseTag.Equals(Other)) {
                phraseTag = GetPhraseTagFromPosTag(leaf.FunctionalTag);
            }

            if (!phraseTag.Equals(Other)) {
                if (isIntermediate) {
                    chunkTag = "I-" + phraseTag;
                } else {
                    chunkTag = "B-" + phraseTag;
                }
            } else {
                chunkTag = phraseTag;
            }

            sentence.Add(leaf.Lexeme);

            tags.Add(leaf.SecondaryTag == null ? leaf.Lexeme : leaf.FunctionalTag);

            target.Add(chunkTag);
        }

        #endregion

    }
}