
using MailKit.Net.Smtp;
using MimeKit;
using SmtpTester.Models;
using SmtpTester.Services.Interfaces;
using System.Diagnostics;

namespace SmtpTester.Services;

public class SmtpTesterService : ISmtpTesterService
{
    public async Task<SmtpTestResponse> TestSmtpAsync(SmtpTestRequest request, CancellationToken cancellationToken = default)
    {
        Stopwatch sw = Stopwatch.StartNew();

        try
        {
            await SendEmail(request, cancellationToken);
        }
        catch (Exception exception)
        {
            sw.Stop();
            return new SmtpTestResponse($"An error occured when sending the email: '{exception.Message}'. See exception for more details.", sw.ElapsedMilliseconds.ToString(), exception);
        }

        sw.Stop();
        return new SmtpTestResponse("Email was sent successfully.", sw.ElapsedMilliseconds.ToString());
    }

    private static async Task SendEmail(SmtpTestRequest request, CancellationToken cancellationToken = default)
    {
        bool sent = false;
        int reTries = 0;
        do
        {
            try
            {
                var email = new MimeMessage();

                // From
                email.From.Add(new MailboxAddress(request.From, request.From));

                // To
                foreach (string address in request.To)
                    email.To.Add(MailboxAddress.Parse(address));

                // Headers
                if (request.Headers != null)
                {
                    foreach (var header in request.Headers)
                        email.Headers.Add(header.Key, header.Value);
                }

                // Content
                var builder = new BodyBuilder();
                email.Sender = new MailboxAddress(request.From, request.From);
                email.Subject = request.Subject;
                builder.HtmlBody = request.Body;

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(request.Host, request.Port, request.SecureSocketOption, cancellationToken);
                await smtp.AuthenticateAsync(request.UserName, request.Password, cancellationToken);
                await smtp.SendAsync(email, cancellationToken);

                sent = true;

                await smtp.DisconnectAsync(true, cancellationToken);
            }
            catch
            {
                if (reTries == 5)
                {
                    // only throw error if 5 retires have occured
                    throw;
                }
                reTries++;
            }
        }
        while (!sent);
    }
}