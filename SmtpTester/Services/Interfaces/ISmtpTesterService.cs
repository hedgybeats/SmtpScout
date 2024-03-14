using SmtpTester.Models;

namespace SmtpTester.Services.Interfaces
{
    public interface ISmtpTesterService
    {
        Task<SmtpTestResponse> TestSmtpAsync(SmtpTestRequest request, CancellationToken cancellationToken = default);
    }
}
