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

namespace SharpNL.Classifier {
    /// <summary>
    /// Represents a class.
    /// </summary>
    /// <typeparam name="T">The feature type.</typeparam>
    public interface IClass<T> : IEnumerable<T>, IEquatable<IClass<T>> {

        /// <summary>
        /// Gets the class name.
        /// </summary>
        /// <value>The class name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the class features.
        /// </summary>
        /// <value>The class features.</value>
        HashSet<T> Features { get; }

        /// <summary>
        /// Determines whether the class contains the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if the class contains the specified feature; otherwise, <c>false</c>.</returns>
        bool Contains(T feature);

    }
}