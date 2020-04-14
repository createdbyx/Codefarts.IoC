// <copyright file="SafeDictionary.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.IoC
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides thread safe dictionary container for fetching and retrieving values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.IDisposable" />
    internal class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly object lockObject = new object();
        private readonly IDictionary<TKey, TValue> internalDictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            set
            {
                lock (this.lockObject)
                {
                    TValue current;
                    if (this.internalDictionary.TryGetValue(key, out current))
                    {
                        var disposable = current as IDisposable;

                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }

                    this.internalDictionary[key] = value;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this.lockObject)
            {
                return this.internalDictionary.TryGetValue(key, out value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (this.lockObject)
            {
                return this.internalDictionary.Remove(key);
            }
        }

        public void Clear()
        {
            lock (this.lockObject)
            {
                this.internalDictionary.Clear();
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.internalDictionary.Keys;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (this.lockObject)
            {
                foreach (var item in this.internalDictionary.Values)
                {
                    var disposable = item as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}