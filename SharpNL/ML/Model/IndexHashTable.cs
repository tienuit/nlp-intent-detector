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

namespace SharpNL.ML.Model {

    /// <summary>
    /// The <see cref="T:IndexHashTable{T}"/> is a hash table which maps entries of an array to their index in the array. 
    /// All entries in the array must be unique otherwise a well-defined mapping is not possible.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public class IndexHashTable<T> : IEquatable<IndexHashTable<T>> {

        private readonly int[] buckets;
        private readonly Slot[] slots;

        private readonly int lastIndex;
        private readonly int count;
        private readonly IEqualityComparer<T> comparer;

        public IndexHashTable(T[] mapping) {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            if (mapping.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(mapping));
            
            comparer = EqualityComparer<T>.Default;
            lastIndex = 0;
            count = 0;

            var freeList = -1;
            var size = GetPrime(mapping.Length);

            buckets = new int[size];
            slots = new Slot[size];

            foreach (var value in mapping) {
                var hashCode = GetHash(value);
                var bucket = hashCode % buckets.Length;
                for (var i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                    if (slots[i].hashCode == hashCode && comparer.Equals(slots[i].value, value))
                        goto next; // the hash already exist... skip

                int index;
                if (freeList >= 0) {
                    index = freeList;
                    freeList = slots[index].next;
                } else {
                    index = lastIndex;
                    lastIndex++;
                }

                slots[index].hashCode = hashCode;
                slots[index].value = value;
                slots[index].next = buckets[bucket] - 1;
                buckets[bucket] = index + 1;
                count++; 
            next:
                ;
            }
        }

        public int this[T item] {
            get {
                var hashCode = GetHash(item);
                for (var i = buckets[hashCode%buckets.Length] - 1; i >= 0; i = slots[i].next) {
                    if ((slots[i].hashCode) == hashCode && comparer.Equals(slots[i].value, item))
                        return i;

                }

                return -1;
            }
        }

        #region . ToArray .
        /// <summary>
        /// Returns a new array containing the contents of the <see cref="IndexHashTable{T}"/>.
        /// </summary>
        /// <returns>A new array containing the contents of the <see cref="IndexHashTable{T}"/>.</returns>
        public T[] ToArray() {
            var array = new T[lastIndex];
            for (var i = 0; i < lastIndex; i++)
                array[i] = slots[i].value;
            
            return array;
        }
        #endregion

        #region + Helper methos .

        // ReSharper disable once StaticMemberInGenericType
        private static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        private static int GetPrime(int min) {
            foreach (var prime in primes)
                if (prime >= min) 
                    return prime;

            for (var i = (min | 1); i < int.MaxValue; i += 2) {
                if (IsPrime(i) && ((i - 1) % 101 != 0))
                    return i;
            }
            return min;
        }

        private static bool IsPrime(int value) {
            if ((value & 1) == 0) 
                return value == 2;

            var limit = (int)Math.Sqrt(value);
            for (var i = 3; i <= limit; i += 2)
                if ((value % i) == 0)
                    return false;
            
            return true;
        }

        private int GetHash(T item) {
            return item == null ? 0 : comparer.GetHashCode(item) & 0x7FFFFFFF;
        }

        #endregion

        #region . Contains .
        /// <summary>
        /// Checks if this hash table contains an entry with the given item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><c>true</c> if the hash table contains an entry with the given item; otherwise, <c>false</c>.</returns>
        public bool Contains(T item) {
            if (buckets == null) 
                return false;

            var hashCode = GetHash(item);
            for (var i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                if (slots[i].hashCode == hashCode && comparer.Equals(slots[i].value, item))
                    return true;                
            
            
            return false;
        }
        #endregion

        #region @ Slot .
        internal struct Slot {
            internal int hashCode;
            internal T value;
            internal int next;
        }
        #endregion

        #region . Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IndexHashTable<T> other) {
            if (other == null) return false;
            if (other.count != count) return false;

            for (var i = 0; i < lastIndex; i++)
                if (slots[i].hashCode > 0 && !other.Contains(slots[i].value))
                    return false;

            return true;
        }
        #endregion

    }
}