namespace Codefarts.IoC
{
    using System;
    using System.Collections.Generic;

    public class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly object lockObject = new object();
        private readonly Dictionary<TKey, TValue> internalDictionary = new Dictionary<TKey, TValue>();

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
                return this.internalDictionary.Keys;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (this.lockObject)
            {
                // var disposableItems = this.internalDictionary.Values.OfType<IDisposable>();

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

        #endregion
    }
}