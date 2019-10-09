using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Logman.Common.Contracts;

namespace Logman.Business.Caching
{
    public class AspNetCaheProvider : ICacheProvider
    {
        private readonly MemoryCache _cache;
        private readonly TimeSpan _defaultLifespan;

        public AspNetCaheProvider()
        {
            _defaultLifespan = new TimeSpan(1, 0, 0, 0);
            _cache = MemoryCache.Default;
        }

        public void Add(string key, object item)
        {
            _cache.Add(key, item, new DateTimeOffset(DateTime.Now, _defaultLifespan));
        }

        public void Add(string key, object item, DateTimeOffset lifeSpan)
        {
            var itemPolicy = new CacheItemPolicy() {AbsoluteExpiration = lifeSpan};
            _cache.Add(key, item, itemPolicy);
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.Run(() => Get<T>(key));
        }

        public Task<T> GetAsync<T>(string key, T defaultValue)
        {
            return Task.Run(() => Get(key, defaultValue));
        }

        public T Get<T>(string key)
        {
            var cachedItem = _cache[key];
            return cachedItem != null ? (T) cachedItem : default(T);
        }

        public T Get<T>(string key, T defaultValue)
        {
            var cachedItem = _cache[key];
            return cachedItem != null ? (T) cachedItem : defaultValue;
        }

        public bool Exists(string key)
        {
            return _cache.Contains(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}