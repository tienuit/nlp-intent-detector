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

namespace SharpNL.Classifier {
    /// <summary>
    /// Interface IClassClassifierResult
    /// </summary>
    /// <typeparam name="C">The result class.</typeparam>
    /// <typeparam name="F">The feature type.</typeparam>
    public interface IClassClassifierResult<C, F> : IComparable<IClassClassifierResult<C, F>> where C : IClass<F> {

        /// <summary>
        /// Gets the result class.
        /// </summary>
        /// <value>The result class.</value>
        C Class { get; }

        /// <summary>
        /// Gets the result probability.
        /// </summary>
        /// <value>The result probability.</value>
        double Probability { get; }

        
    }
}