using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logman.Common.Contracts
{
    public interface IEmailProvider
    {
        Task  SendAsync(string from, string to, string subject, string body, Dictionary<string,string> imagePaths=null );
    }
}