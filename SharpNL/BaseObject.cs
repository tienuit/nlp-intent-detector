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

namespace SharpNL {
    /// <summary>
    /// Provides the basic properties for data holder objects.
    /// </summary>
    public abstract class BaseObject {

        #region + Properties .

        #region . Data .

        private Dictionary<string, object> data;

        /// <summary>
        /// Gets the data dictionary stored on this instance.
        /// </summary>
        /// <value>The data dictionary stored on this instance.</value>
        public Dictionary<string, object> Data => data ?? (data = new Dictionary<string, object>());

        #endregion

        #region . HasData .
        /// <summary>
        /// Gets a value indicating whether this instance has data.
        /// </summary>
        /// <value><c>true</c> if this instance has data; otherwise, <c>false</c>.</value>
        public bool HasData => (data != null && data.Count > 0);

        #endregion

        #endregion

        #region + Methods .

        #region . GetData .
        /// <summary>
        /// Retrieves the value associated with the specified <paramref name="key" />. If the key is not found,
        /// returns a default value that you provide.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="key">The data key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value associated with <paramref name="key" />, or <paramref name="defaultValue" /> if <paramref name="key" /> is not found.</returns>
        /// <exception cref="System.InvalidOperationException">The specified return type is not valid!</exception>
        public T GetData<T>(string key, T defaultValue) {
            if (data == null || !data.ContainsKey(key))
                return defaultValue;

            if (!(data[key] is T))
                throw new InvalidOperationException("The specified return type is not valid!");

            return (T)data[key];
        }
        #endregion

        #region . TryGetData .
        /// <summary>
        /// Attempts to get data from the current instance and returns a value indicating if the operation succeeded.
        /// </summary>
        /// <param name="key">The data key.</param>
        /// <param name="value">The data value.</param>
        /// <returns><c>true</c> if the operation succeeded, <c>false</c> otherwise.</returns>
        public bool TryGetData<T>(string key, out T value) {
            if (data == null || !data.ContainsKey(key)) {
                value = default(T);
                return false;
            }
            value = (T)data[key];
            return true;
        }
        #endregion

        #endregion

    }
}