using System.Threading;
using System.Threading.Tasks;

namespace TasteOfHome.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default);
    }
}