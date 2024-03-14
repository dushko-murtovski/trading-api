using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace StockWolfTradingAPI.Common
{
    public class Email
    {
        private string from = $"test@example.com";
        private string subject;
        private string to;
        private string plainTextContent;
        private string htmlContent;

        readonly SendGridClient client;

        public Email()
        {
            var apiKey = "";
            from = "info@stockwolftrading.com";
            client = new SendGridClient(apiKey);
        }

        public string Subject { get => subject; set => subject = value; }
        public string To { get => to; set => to = value; }
        public string PlainTextcontent { get => plainTextContent; set => plainTextContent = value; }
        public string HtmlContent { get => htmlContent; set => htmlContent = value; }
        public string From { get => from; set => from = value; }

        public async Task<Response> ExecuteUserActivated(string bodyString)
        {
            var fromAddr = new EmailAddress(from, "Info");
            subject = "Stock wolf trading – account verification";
            var toAddr = new EmailAddress(to, "User");
            plainTextContent = "";
            htmlContent = bodyString;
            var msg = MailHelper.CreateSingleEmail(fromAddr, toAddr, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }

        public async Task<Response> SendMessageEmail(string name, string email, string message)
        {
            var fromAddr = new EmailAddress(email, name);
            subject = "Web site message";
            var toAddr = new EmailAddress(to, "User");
            plainTextContent = message;
            htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(fromAddr, toAddr, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
