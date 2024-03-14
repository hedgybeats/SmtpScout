using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmtpTester.Models;
using SmtpTester.Services.Interfaces;

namespace SmtpTester.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmtpTester(ISmtpTesterService smtpTesterService) : ControllerBase
    {
        private readonly ISmtpTesterService _smtpTesterService = smtpTesterService;

        [HttpPost]
        [OpenApiOperation("Test a given SMTP configuration.",
                          "From: The email from which the email will be sent.\n\n" +
                           "Host: The SMTP host to use.\n\n" +
                           "SecureSocketOptions (optional): The secure socket option to use, defaults to StartTls.\n\n" +
                           "Username: The username to use when authenticating.\n\n" +
                           "Password: The password to use when authenticating.\n\n" +
                           "To: A list of the emails to send the test email to.\n\n" +
                           "Subject (optional): The subject to used in the test email.\n\n" +
                           "Body (optional): The body to used in the test email.\n\n" +
                           "Headers (optional): Custom headers to be added to the email request.\n\n")]
        public async Task<SmtpTestResponse> TestSmtp(SmtpTestRequest request, CancellationToken cancellationToken = default)
        {
            return await _smtpTesterService.TestSmtpAsync(request, cancellationToken);
        }
    }
}
