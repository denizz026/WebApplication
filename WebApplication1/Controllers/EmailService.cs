using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _emailFrom;
    private readonly string _emailPassword;

    public EmailService(string smtpHost, int smtpPort, string emailFrom, string emailPassword)
    {
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
        _emailFrom = emailFrom;
        _emailPassword = emailPassword;
    }

    public async Task SendEmailAsync(string emailTo, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("CinemaReview", _emailFrom));
        email.To.Add(new MailboxAddress("", emailTo));
        email.Subject = subject;
        email.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_emailFrom, _emailPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}