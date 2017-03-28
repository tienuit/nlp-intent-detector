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

using System.Collections.Generic;

namespace SharpNL.Classifier {
    /// <summary>
    /// Represents a class classifier.
    /// </summary>
    /// <typeparam name="C">The class type.</typeparam>
    /// <typeparam name="R">The result type.</typeparam>
    /// <typeparam name="F">The feature type.</typeparam>
    public interface IClassClassifier<C, R, in F> : IEnumerable<C>
        where R : IClassClassifierResult<C, F>
        where C : IClass<F> {

        /// <summary>
        /// Gets the classes in this classifier.
        /// </summary>
        /// <value>The classes in this classifier.</value>
        HashSet<C> Classes { get; }
            
        /// <summary>
        /// Classifies the specified <paramref name="features"/>.
        /// </summary>
        /// <param name="features">The features to be classified.</param>
        /// <returns>The classification values for the features, the higher, the better is the match.</returns>
        SortedSet<R> Classify(F[] features);

    }
}