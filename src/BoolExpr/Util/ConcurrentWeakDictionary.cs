using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BoolExprNet.Util
{

    /// <summary>
    /// Provides a thread-safe dictionary that holds <see cref="WeakReference"/>s to its contained values. Objects
    /// added as values are free to be collected by the GC. Periodically performs a compact to remove stale reference
    /// objects.
    /// </summary>
    /// <remarks>Implementation does not currently support <c>null</c> values.</remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    class ConcurrentWeakDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>
        where TValue : class
    {

        const int MIN_CLEAN_INTERVAL = 500;

        readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        readonly Dictionary<TKey, WeakReference<TValue>> storage = new Dictionary<TKey, WeakReference<TValue>>();
        volatile int version;
        volatile int cleanVersion;
        volatile int cleanGeneration;

        /// <summary>
        /// Attempts to return the value of the given reference. Returns <c>null</c> if the object has been released
        /// by the GC.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        T Dereference<T>(WeakReference<T> reference)
            where T : class
        {
            return reference != null ? reference.TryGetTarget(out T o) ? o : null : null;
        }

        /// <summary>
        /// Gets the keys in the dictionary.
        /// </summary>
        /// <remarks>
        /// Implementation performs a full copy of the underlying keys collection.
        /// </remarks>
        public ICollection<TKey> Keys
        {
            get => sync.WithReadLock(() => storage.Keys.ToList());
        }

        /// <summary>
        /// Gets the values in the dictionary.
        /// </summary>
        /// <remarks>
        /// Implementation performs a full copy of the underlying values collection.
        /// </remarks>
        public ICollection<TValue> Values
        {
            get => sync.WithReadLock(() => storage.Values.Select(i => Dereference(i)).Where(i => i != null).ToList());
        }

        /// <summary>
        /// Gets or sets the item with the specified key. Returns <c>null</c> if the specified value does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get => sync.WithReadLock(() => Dereference(storage.GetOrDefault(key)));
            set => sync.WithWriteLock(() => storage[key] = new WeakReference<TValue>(value));
        }

        /// <summary>
        /// Attempts to get the value of the specified key if it exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            AutoCompact(1);

            using (sync.BeginReadLock())
                return (value = this[key]) != null;
        }

        /// <summary>
        /// Adds a key/value pair into the <see cref="ConcurrentWeakDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AutoCompact(2);

            return sync.WithUpgradableReadLock(() => this[key] ?? (this[key] = value));
        }

        /// <summary>
        /// Adds a key/value pair into the <see cref="ConcurrentWeakDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            AutoCompact(2);

            return sync.WithUpgradableReadLock(() => this[key] ?? (this[key] = valueFactory(key)));
        }

        /// <summary>
        /// Uses the specified functions to add a key/value pair to the <see
        /// cref="ConcurrentWeakDictionary{TKey, TValue}"/> if the key does not already exist, or to update a
        /// key/value pair in the <see cref="ConcurrentWeakDictionary{TKey, TValue}"/> if the key already exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addValue"></param>
        /// <param name="updateValueFactory"></param>
        public void AddOrUpdate(TKey key, Func<TKey, TValue> addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (addValue == null)
                throw new ArgumentNullException(nameof(addValue));
            if (updateValueFactory == null)
                throw new ArgumentNullException(nameof(updateValueFactory));

            AutoCompact(2);

            using (sync.BeginUpgradableReadLock())
            {
                var v = this[key];
                this[key] = v == null ? addValue(key) : updateValueFactory(key, v);
            }
        }

        /// <summary>
        /// Attempts to add the specified key and value to the <see cref="ConcurrentWeakDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            AutoCompact(2);

            return sync.WithUpgradableReadLock(() => (this[key] = this[key] == null ? value : null) != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (newValue == null)
                throw new ArgumentNullException(nameof(newValue));
            if (comparisonValue == null)
                throw new ArgumentNullException(nameof(comparisonValue));

            AutoCompact(2);

            using (sync.BeginUpgradableReadLock())
            {

                var v = this[key];
                if (v != null && v == comparisonValue)
                {
                    this[key] = newValue;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Empties all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            using (sync.BeginWriteLock())
            {
                storage.Clear();
                version = cleanVersion = cleanGeneration = 0;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the specified key exists in the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return this[key] != null;
        }

        /// <summary>
        /// Removes the item with the specified key from the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            AutoCompact(1);

            using (sync.BeginUpgradableReadLock())
            {
                if ((value = this[key]) != null)
                {
                    using (sync.BeginWriteLock())
                    {
                        storage.Remove(key);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the number of items in the dictionary. This method is inaccurate as it does not track collected
        /// objects until a cleanup is run.
        /// </summary>
        public int Count => sync.WithReadLock(() => storage.Count);

        /// <summary>
        /// Returns <c>true</c> if items cannot be added to this collection.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>This method obtains a read lock for the duration of a complete enumeration of the collection.</remarks>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return sync.WithReadLock(() => storage
                .Select(i => new KeyValuePair<TKey, TValue>(i.Key, Dereference(i.Value)))
                .Where(i => i.Value != null)
                .ToList()
                .GetEnumerator());
        }

        /// <summary>
        /// Initiates a cleanup if the object has been modified enough.
        /// </summary>
        /// <param name="incVersion"></param>
        internal void AutoCompact(int incVersion)
        {
            // increment outside of read-lock
            var v = Interlocked.Add(ref version, incVersion);

            using (sync.BeginUpgradableReadLock())
            {
                // compact the storage every so often
                var l = (long)v - cleanVersion;
                if (l > MIN_CLEAN_INTERVAL + storage.Count)
                {
                    using (sync.BeginWriteLock())
                    {
                        // a cleanup will be useless unless a GC has happened in the meantime
                        // WeakReferences can become empty only during the GC
                        int curGeneration = GC.CollectionCount(0);
                        if (cleanGeneration != curGeneration)
                        {
                            // purge empty items from storage
                            foreach (var k in storage
                                    .Where(i => Dereference(i.Value) == null)
                                    .Select(i => i.Key)
                                    .ToList())
                                storage.Remove(k);

                            cleanGeneration = curGeneration;
                            cleanVersion = v;
                        }
                        else
                            cleanVersion = MIN_CLEAN_INTERVAL;
                    };
                }
            };
        }

        #region IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (!TryAdd(item.Key, item.Value))
                throw new ArgumentException(nameof(item));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < storage.Count)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            sync.WithReadLock(() => storage
                .Select(i => new KeyValuePair<TKey, TValue>(i.Key, Dereference(i.Value)))
                .Where(i => i.Value != null)
                .ToList()) // TODO inefficient
                .CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return TryRemove(item.Key, out TValue o);
        }

        #endregion

        #region IDictionary<TKey, TValue>

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException(nameof(key));
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return TryRemove(key, out TValue o);
        }

        #endregion

    }

}
