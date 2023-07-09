using Ordering_API.Entities;
using System.Threading.Tasks;

namespace Ordering_API.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
