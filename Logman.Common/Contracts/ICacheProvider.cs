using System;
using System.Threading.Tasks;

namespace Logman.Common.Contracts
{
    public interface ICacheProvider
    {
        void Add(string key, object item);
        void Add(string key, object item, DateTimeOffset lifeSpan);

        Task<T> GetAsync<T>(string key);
        Task<T> GetAsync<T>(string key, T defaultValue);

        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);

        bool Exists(string key);

        void Remove(string key);
    }
}