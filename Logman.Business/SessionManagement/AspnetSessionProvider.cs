using System.Web;
using System.Web.SessionState;
using Logman.Common.Contracts;

namespace Logman.Business.SessionManagement
{
    public class AspnetSessionProvider : ISessionProvider
    {
        private readonly HttpSessionState _session;

        public AspnetSessionProvider()
        {
            _session = HttpContext.Current.Session;
        }

        public object this[string key]
        {
            get { return _session[key]; }
            set { _session[key] = value; }
        }

        public T GetTyped<T>(string key)
        {
            return (T) this[key];
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }
    }
}