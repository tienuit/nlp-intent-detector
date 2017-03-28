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
using System.Globalization;
using SharpNL.Globalization.Cultures;
using SharpNL.Utility;

namespace SharpNL.Globalization {
    /// <summary>
    /// Represents a abstract culture.
    /// </summary>
    public abstract class Culture : IStopwordProvider {

        #region + Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="Culture"/> class.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="cultureName"/>.
        /// </exception>
        /// <exception cref="CultureNotFoundException" />
        protected Culture(string cultureName) {

            if (string.IsNullOrEmpty(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            CultureInfo = CultureInfo.GetCultureInfo(cultureName);
        }
        #endregion

        #region + Properties .

        #region . CultureInfo .
        /// <summary>
        /// Gets the culture information.
        /// </summary>
        /// <value>The culture information.</value>
        public CultureInfo CultureInfo { get; private set; }
        #endregion

        #region . Stopwords .
        /// <summary>
        /// Gets the a set with all the stopwords.
        /// </summary>
        /// <value>The set with all the stopwords.</value>
        public HashSet<string> Stopwords { get; protected set; }
        #endregion

        #endregion

        #region . GetCulture .
        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        /// <returns>Culture.</returns>
        /// <exception cref="System.ArgumentNullException">cultureName</exception>
        public static Culture GetCulture(string cultureName) {

            if (string.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            switch (cultureName.ToLowerInvariant()) {
                case "en":
                    return en.Instance;
                case "pt-br":
                case "pt_br":
                    return pt_BR.Instance;
                default:
                    return new GenericCulture(cultureName);
            }
        }
        #endregion

        #region . IsStopword .
        /// <summary>
        /// Determines whether the specified word is a stopword.
        /// </summary>
        /// <param name="word">The word to evaluate.</param>
        /// <returns><c>true</c> if the specified word is a stopword; otherwise, <c>false</c>.</returns>
        public virtual bool IsStopword(string word) {
            if (string.IsNullOrWhiteSpace(word))
                return true;

            if (word.Length == 1 && char.IsPunctuation(word[0]))
                return true;

            return Stopwords != null && Stopwords.Contains(word);
        }
        #endregion IsStopword
        
    }
}