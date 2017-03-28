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
using System.Text;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents an lemmatized sentence.
    /// </summary>
    public class LemmaSample {
        /// <summary>
        /// Construct the lemma sample.
        /// </summary>
        /// <param name="tokens">The sample tokens.</param>
        /// <param name="tags">The POS tags.</param>
        /// <param name="lemmas">The lemmas.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tokens" />, <paramref name="tags" /> or
        /// <paramref name="lemmas" />.
        /// </exception>
        /// <exception cref="ArgumentException">All the arguments must have the same length.</exception>
        public LemmaSample(string[] tokens, string[] tags, string[] lemmas) {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            if (lemmas == null)
                throw new ArgumentNullException(nameof(tags));

            if (tokens.Length != tags.Length || tags.Length != lemmas.Length)
                throw new ArgumentException("All the arguments must have the same length.");

            Tokens = tokens;
            Tags = tags;
            Lemmas = lemmas;
        }

        /// <summary>
        /// Gets the tokens of the sample.
        /// </summary>
        public string[] Tokens { get; private set; }

        /// <summary>
        /// Gets the POS tags of the tokens.
        /// </summary>
        public string[] Tags { get; private set; }

        /// <summary>
        /// Gets the lemmas in the sample.
        /// </summary>
        public string[] Lemmas { get; private set; }

        /// <summary>
        /// Gets the length of the sample.
        /// </summary>
        public int Length => Lemmas.Length;

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() {
            var sb = new StringBuilder();

            for (var i = 0; i < Lemmas.Length; i++) {
                sb.AppendFormat("{0} {1} {2}\n", Tokens[i], Tags[i], Lemmas[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if the other sample is equal to this instance, <c>false</c> otherwise.</returns>
        protected bool Equals(LemmaSample other) {
            return Equals(Tokens, other.Tokens) && Equals(Tags, other.Tags) && Equals(Lemmas, other.Lemmas);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LemmaSample) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = Tokens != null ? Tokens.GetHashCode() : 0;
                hashCode = (hashCode*397) ^ (Tags != null ? Tags.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Lemmas != null ? Lemmas.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}