namespace Logman.Common.Contracts
{
    public interface ISessionProvider
    {
        object this[string key] { get; set; }

        T GetTyped<T>(string key);
        void Remove(string key);
    }
}