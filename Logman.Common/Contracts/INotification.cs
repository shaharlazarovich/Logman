using System.Threading.Tasks;

namespace Logman.Common.Contracts
{
    public interface INotification
    {
        Task SendEmailAsync(string toAddress, string body);
        Task CallWebHookAsync(string url);
    }
}