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

using System.Globalization;

namespace SharpNL.Globalization {
    /// <summary>
    /// Represents a generic/not implemented culture. This class cannot be inherited.
    /// </summary>
    public sealed class GenericCulture : Culture {

        /// <summary>
        /// Initializes a new instance of the <see cref="Culture"/> class.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="cultureName"/>.
        /// </exception>
        /// <exception cref="CultureNotFoundException" />
        public GenericCulture(string cultureName) : base(cultureName) {
            
        }
    }
}