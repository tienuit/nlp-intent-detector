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
using SharpNL.Properties;

namespace SharpNL.Globalization.Cultures {
    /// <summary>
    /// Represents a Brazilian Portuguese culture object. This class cannot be inherited.
    /// </summary>
    public sealed class pt_BR : Culture {

        private static pt_BR instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static pt_BR Instance => instance ?? (instance = new pt_BR());

        private pt_BR() : base("pt_BR") {
            Stopwords = new HashSet<string>(Resources.pt_stopwords.Split(new []{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}