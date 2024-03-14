using System.Text.Json;

namespace SmtpTester.Models;

public class SmtpTestResponse
{
    public SmtpTestResponse(string message, string elapsedMilliseconds)
    {
        Message = message;
        ElapsedMilliseconds = elapsedMilliseconds;
    }

    public SmtpTestResponse(string message, string elapsedMilliseconds, Exception exception)
    {
        Message = message;
        ElapsedMilliseconds = elapsedMilliseconds;
        Exception = JsonSerializer.Serialize(exception);
    }

    public string Message { get; set; }
    public string ElapsedMilliseconds { get; set; }
    public string? Exception { get; set; }

}