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
using System.Collections.Concurrent;
using System.Linq;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a delegate, which resolves the type name into a <see cref="Type" /> object..
    /// </summary>
    /// <param name="type">The string type.</param>
    /// <returns>The <see cref="Type" /> object or a <c>null</c> value if not recognized.</returns>
    public delegate Type ReadTypeDelegate(string type);


    /// <summary>
    /// Represents a delegate, which resolves the type name by the given <paramref name="type"/> object.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The resolved type name, or a <c>null</c> value if it can not be resolved.</returns>
    public delegate string WriteTypeDelegate(Type type);

    /// <summary>
    /// The type resolver is responsible to translate a string representation of a type into a <see cref="Type"/> 
    /// object and vice versa.
    /// </summary>
    public class TypeResolver : Disposable {

        private readonly ConcurrentDictionary<string, Type> types;

        public TypeResolver() {
            types = new ConcurrentDictionary<string, Type>();
        }

        #region + IsRegistered .

        /// <summary>
        /// Determines whether the specified type name is registered.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        public bool IsRegistered(string name) {
            return types.ContainsKey(name);
        }

        /// <summary>
        /// Determines whether the specified type is registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is registered; otherwise, <c>false</c>.</returns>
        public bool IsRegistered(Type type) {
            return types.Any(x => x.Value == type);
        }

        #endregion

        #region . Overwrite .

        /// <summary>
        /// Overwrites an specified type.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <param name="type">The type value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/>
        /// or
        /// <paramref name="type"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The specified name is not registered.</exception>
        /// <remarks>This method locks this entire instance! Use it wisely.</remarks>
        public void Overwrite(string name, Type type) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // this also prevents people to use this function as a set register.
            if (!types.ContainsKey(name))
                throw new ArgumentException("The specified name is not registered.");

            types[name] = type;

        }

        #endregion

        #region . Register .

        /// <summary>
        /// Registers the specified type object with its string representation.
        /// </summary>
        /// <param name="name">A string representation of the given type.</param>
        /// <param name="type">The type.</param>
        public void Register(string name, Type type) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!types.TryAdd(name, type))
                throw new ArgumentException("The specified name is already registered.");

        }

        #endregion

        #region . ResolveType .
        /// <summary>
        /// Resolves the type name into a <see cref="Type" /> object.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <returns>The <see cref="Type" /> object or a <c>null</c> value if not recognized.</returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public Type ResolveType(string name) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type type;
            return types.TryGetValue(name, out type) ? type : null;
        }
        #endregion

        #region . ResolveName .
        /// <summary>
        /// Resolves the type name by the given <paramref name="type"/> object.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <returns>The resolved type name, or a <c>null</c> value if it can not be resolved.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public string ResolveName(Type type) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return (from t in types where t.Value == type select t.Key).FirstOrDefault();
        }
        #endregion
    }
}