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
using System.Collections.Generic;
using System.Linq;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.Extensions {
    /// <summary>
    /// Provide extension methods for arrays.
    /// </summary>
    public static class ArrayExtensions {

        #region . Add .
        /// <summary>
        /// Adds the specified <paramref name="value"/> to the array.
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The new element.</param>
        /// <returns>The original array with the new element.</returns>
        public static T[] Add<T>(this T[] array, T value) {
            if (ReferenceEquals(value, null))
                return array;

            if (array == null || array.Length == 0)
                return new[] { value };

            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
            return array;
        }
        #endregion


        #region . GetArrayHash .
        /// <summary>
        /// Gets the hash code for the contents of the array since the default hash code for an array is unique even if the contents are the same.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to generate a hash code for.</param>
        /// <returns>The hash code for the values in the array.</returns>
        internal static int GetArrayHash<T>(this T[] array) {
            if (array == null)
                return 0;

            unchecked {
                var hash = 17;

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var i in array)
                    hash = hash * 23 + (i != null ? i.GetHashCode() : 0);

                return hash;
            }
        }
        #endregion

        #region . SequenceEqual .
        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first enumerable.</param>
        /// <param name="second">The second enumerable.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns><c>true</c> if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type, <c>false</c> otherwise.</returns>
        public static bool SequenceEqual<T>(this T[] first, T[] second, IEqualityComparer<T> comparer = null) {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            if (first.Length != second.Length)
                return false;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < first.Length; i++) {
                if (!comparer.Equals(first[i], second[i]))
                    return false;
            }
            
            return true;
        }
        #endregion

    }
}