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

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a collection of objects as a <see cref="IObjectStream{I}"/>
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "CA is using drugs! The IDisposable is implemented properly.")]
    public class CollectionObjectStream<T> : Disposable, IObjectStream<T> {
        private IEnumerator<T> enumerator;
        private IEnumerable<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionObjectStream{T}"/> with only one object.
        /// </summary>
        /// <param name="single">A single object.</param>
        public CollectionObjectStream(T single) : this(new [] {single}) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionObjectStream{T}"/> with the specified objects.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public CollectionObjectStream(IEnumerable<T> enumerable) {
            items = enumerable as T[] ?? enumerable.ToArray();
            Reset();
        }

        #region . DisposeManagedResources .

        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected override void DisposeManagedResources() {
            base.DisposeManagedResources();

            // useless ?
            enumerator = null;
            items = null;
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns, null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>The next object or null to signal that the stream is exhausted.</returns>
        /// <exception cref="System.ObjectDisposedException">The CollectionObjectStream instance has been disposed and can no longer be used for operations.</exception>
        public T Read() {
            if (items == null || enumerator == null)
                throw new ObjectDisposedException(
                    "The CollectionObjectStream instance has been disposed and can no longer be used for operations.");

            if (enumerator.MoveNext()) {
                return enumerator.Current;
            }
            return default(T);
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The CollectionObjectStream instance has been disposed and can no longer be used for operations.</exception>
        public void Reset() {
            if (items == null)
                throw new ObjectDisposedException(
                    "The CollectionObjectStream instance has been disposed and can no longer be used for operations.");

            enumerator = items.GetEnumerator();
        }

        #endregion

    }
}