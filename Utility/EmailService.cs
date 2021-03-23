using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace TestTwinCoreProject.Utility
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email,string subject, string message)
        {

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта Nekotik ", "nekotik_kotik@ukr.net"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.ukr.net", 2525, true);
                await client.AuthenticateAsync("nekotik_kotik@ukr.net", "ugPwTkUNZ5vBcEX7");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
