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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using SharpNL.Utility;
using Ver = SharpNL.Utility.Version;
using Version = System.Version;

namespace SharpNL {
    /// <summary>
    /// Represents the SharpNL library.
    /// </summary>
    public static class Library {
        private static readonly object syncLock = new object();
        private static List<Type> knownTypes;

        /// <summary>
        /// Initializes static members of the SharpNL library.
        /// </summary>
        static Library() {
            Version = typeof(Library).Assembly.GetName().Version;


            MinOpenNLPVersion = new Ver(1, 5, 0, false);
            MaxOpenNLPVersion = new Ver(1, 6, 0, false);

            LoadKnownTypes();

            TypeResolver = new TypeResolver();
           
            foreach (var type in knownTypes) {
                var attr = type.GetCustomAttribute<TypeClassAttribute>(false);
                if (attr != null) {
                    TypeResolver.Register(attr.Name, type);
                }
            }

            langCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                // preloaded languages in the cache
                {"en", "en"},
                {"pt-PT","pt"},
                {"pt-BR", "pt"}
            };
        }

        #region + Properties .

        #region . MinOpenNLPVersion .
        /// <summary>
        /// Gets the minimum supported version of OpenNLP.
        /// </summary>
        /// <value>The minimum supported version of OpenNLP</value>
        public static Ver MinOpenNLPVersion { get; private set; }
        #endregion

        #region . MaxOpenNLPVersion .
        /// <summary>
        /// Gets the highest supported OpenNLP version.
        /// </summary>
        /// <value>The supported OpenNLP version.</value>
        public static Ver MaxOpenNLPVersion { get; private set; }
        #endregion

        #region . Version .

        /// <summary>
        /// Gets the version of the SharpNL library.
        /// </summary>
        /// <value>The version of the SharpNLP library.</value>
        public static Version Version { get; private set; }

        #endregion

        #region . TypeResolver .
        /// <summary>
        /// Gets type resolver loaded for this library instance.
        /// </summary>
        /// <value>The type resolver.</value>
        public static TypeResolver TypeResolver { get; }
        #endregion

        #endregion

        #region . Donate .
        /// <summary>
        /// Donate to the library author.
        /// </summary>
        public static void Donate() {
            try {
                Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7SWNPAPJNSARC");
            } catch {
                // ignored
            }
        }
        #endregion

        #region . GetInstance .
        /// <summary>
        /// Gets the instance for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The instance of the given type.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        internal static object GetInstance(Type type) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var instance = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            return instance != null 
                ? instance.GetValue(null) 
                : Activator.CreateInstance(type);
        }
        internal static T GetInstance<T>(Type type) {
            return (T)GetInstance(type);
        }
        #endregion

        #region . GetKnownTypes .
        /// <summary>
        /// Gets the known types that inherits the given <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        internal static IEnumerable<Type> GetKnownTypes(Type baseType) {
            // this lock is not released until the last yield is processed.
            lock (syncLock) {
                foreach (var type in knownTypes) {
                    if (baseType.IsInterface) {
                        if (type.IsAssignableFrom(baseType))
                            yield return type;
                    } else if (type.IsSubclassOf(baseType)) {
                        yield return type;
                    }
                }
            }
        }
        #endregion

        #region . GetLang .
        private static readonly Dictionary<string, string> langCache;

        /// <summary>
        /// Gets the ISO 639-1 two-letter code for the language of the specified <paramref name="cultureName"/>.
        /// </summary>
        /// <param name="cultureName">The name of a culture.</param>
        /// <returns>The ISO 639-1 two-letter code for the language.</returns>
        internal static string GetLang(string cultureName) {
            if (string.IsNullOrEmpty(cultureName))
                return null;

            lock (langCache) {
                if (langCache.ContainsKey(cultureName))
                    return langCache[cultureName];

                var info = CultureInfo.GetCultureInfo(cultureName);

                langCache[cultureName] = info.TwoLetterISOLanguageName;

                return langCache[cultureName];
            }           
        }
        #endregion

        #region . CurrentTimeMillis .
        internal static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Currents the current time in millis.
        /// </summary>
        /// <returns>System.Int64.</returns>
        internal static long CurrentTimeMillis() {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
        #endregion

        #region . LoadKnownTypes .
        /// <summary>
        /// Loads the known types from the assemblies in the current application domain.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal static void LoadKnownTypes() {
            lock (syncLock) {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                knownTypes = new List<Type>();
                foreach (var assembly in assemblies) {
                    var modules = assembly.GetModules();
                    foreach (var module in modules) {
                        try {
                            knownTypes.AddRange(module.GetTypes());
                        } catch (ReflectionTypeLoadException e) {
                            foreach (var type in e.Types) {
                                if (type != null)
                                    knownTypes.Add(type);
                            }
                        } catch (FileNotFoundException) {
                            // Knuppe 2016-01-15: 
                            // Some libraries on Linux are loaded dynamically, for them the GetTypes function 
                            // rises a FileNotFoundException (at this point we dont't care about these libraries);
                        }
                    }
                }
            }
        }
        #endregion

    }
}