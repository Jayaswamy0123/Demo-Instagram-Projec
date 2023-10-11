using Instagram.Repository.Iservices;
using Microsoft.AspNetCore.Diagnostics;
using static Instagram.GlobalVariables.ResponseConstents;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Instagram.Repository.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        public EmailServices(IConfiguration configuration,
                             IWebHostEnvironment environment)
        {
            _configuration = configuration;      /*initialize the _configuration and _environment
                                                 fields with the values passed as parameters to the constructor. This is known as constructor initialization.*/
            _environment = environment;
        }

        public async Task ForgotPasswordEmail(string email, string url)
        {
            string template = await ReadTemplate(TemplateName.ForgotPassword);
            string content = template.Replace("{URL}", url);
            await SendMultipleEmail(EmailSubject.ForgotPassword, email, content);
        }


        public async Task ConfirmationMail(string email, string url)
        {
            string template = await ReadTemplate(TemplateName.AccoutConfirmationMail);
            string content = template.Replace("{URL}", url);
            await SendMultipleEmail(EmailSubject.AccountCreation, email, content);
        }

        public async Task<string> ReadTemplate(string templateName)
        {
            string pathToFile = $"{Directory.GetCurrentDirectory()}/Templates{Path.DirectorySeparatorChar}{templateName}";
            string builder = "";
            using (StreamReader reader = File.OpenText(pathToFile))
            {
                builder = await reader.ReadToEndAsync();
            }
            return builder;
        }


        public async Task SendExceptionMail(IExceptionHandlerFeature ex, HttpContext context)
        {
            var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.Source}<hr />{context.Request.Path}<br />";

            var code = context.Response.StatusCode;
            err += $"Query Parameters if present :- {context.Request.QueryString}<hr/>";
            err += $"Response Status Code :- {code}<hr/>";
            err += $"Time :- {DateTime.UtcNow} <hr/>";
            err += $"BaseUrl :- {context.Request.Host.Host}<hr/>";

            context.Request.EnableBuffering();

            // Leave the body open so the next middleware can read it.
            using (var reader = new System.IO.StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                err += $"Payload :- {body}<hr/>";
            }

            string ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string ipAddress = context.Connection.RemoteIpAddress.ToString();
            string ipPort = context.Connection.RemotePort.ToString();
            err += $"ip :- {ip} , ipAddress :- {ipAddress} , ipPort :- {ipPort} <hr />";

            var userAgent = context.Request.Headers["User-Agent"].ToString();
            err += $"userAgent :- {userAgent}<hr/>";

            string strHostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(strHostName).GetValue(1).ToString();
            err += $" Clinet Ip :- {clientIPAddress}<hr/>";

            err += $"Host :- {strHostName}<hr/>";

            string user = userAgent.ToLower();
            string browser = "";

            string os;
            //=================OS=======================
            if (userAgent.ToLower().Contains("windows", StringComparison.CurrentCulture))
            {
                os = "Windows";
            }
            else if (userAgent.ToLower().Contains("mac", StringComparison.CurrentCulture))
            {
                os = "Mac";
            }
            else if (userAgent.ToLower().Contains("x11", StringComparison.CurrentCulture))
            {
                os = "Unix";
            }
            else if (userAgent.ToLower().Contains("android", StringComparison.CurrentCulture))
            {
                os = "Android";
            }
            else if (userAgent.ToLower().Contains("iphone", StringComparison.CurrentCulture))
            {
                os = "IPhone";
            }
            else
            {
                os = "UnKnown, More-Info: " + userAgent;
            }
            //===============Browser===========================
            if (user.Contains("msie"))
            {
                string Substring = userAgent[userAgent.IndexOf("MSIE")..].Split(";")[0];
                browser = Substring.Split(" ")[0].Replace("MSIE", "IE") + "-" + Substring.Split(" ")[1];
            }
            else if (user.Contains("safari") && user.Contains("version"))
            {
                browser = (userAgent[userAgent.IndexOf("Safari")..].Split(" ")[0]).Split("/")[0] + "-" + (userAgent[userAgent.IndexOf("Version")..].Split(" ")[0]).Split("/")[1];
            }
            else if (user.Contains("opr") || user.Contains("opera"))
            {
                if (user.Contains("opera"))
                    browser = (userAgent[userAgent.IndexOf("Opera")..].Split(" ")[0]).Split("/")[0] + "-" + (userAgent[userAgent.IndexOf("Version")..].Split(" ")[0]).Split("/")[1];
                else if (user.Contains("opr"))
                    browser = ((userAgent[userAgent.IndexOf("OPR")..].Split(" ")[0]).Replace("/", "-")).Replace("OPR", "Opera");
            }
            else if (user.Contains("chrome"))
            {
                browser = (userAgent[userAgent.IndexOf("Chrome")..].Split(" ")[0]).Replace("/", "-");
            }
            else if ((user.IndexOf("mozilla/7.0") > -1) || (user.IndexOf("netscape6") != -1) || (user.IndexOf("mozilla/4.7") != -1) || (user.IndexOf("mozilla/4.78") != -1) || (user.IndexOf("mozilla/4.08") != -1) || (user.IndexOf("mozilla/3") != -1))
            {
                browser = "Netscape-?";

            }
            else if (user.Contains("firefox"))
            {
                browser = (userAgent[userAgent.IndexOf("Firefox")..].Split(" ")[0]).Replace("/", "-");
            }
            else if (user.Contains("rv"))
            {
                browser = string.Concat("IE-", user.AsSpan(user.IndexOf("rv") + 3, user.IndexOf(")")));
            }
            else
            {
                browser = "UnKnown, More-Info: " + userAgent;
            }

            err += $"Os :- {os}<hr/>";
            err += $"Browser :- {browser}<hr/>";

            err += $"Stack Trace<hr />{ex.Error.StackTrace.Replace(Environment.NewLine, "<br />")}";

            if (ex.Error.InnerException != null)
                err +=
                    $"Inner Exception<hr />{ex.Error.InnerException?.Message.Replace(Environment.NewLine, "<br />")}";
            // This bit here to check for a form collection!

            if (context.Request.HasFormContentType && context.Request.Form.Any())
            {
                err += "<table border=\"1\"><tr><td colspan=\"2\">Form collection:</td></tr>";
                foreach (var form in context.Request.Form)
                {
                    err += $"<tr><td>{form.Key}</td><td>{form.Value}</td></tr>";
                }
                err += "</table>";
            }


            var email = _configuration.GetValue<string>("ContactUs:Email");
            List<string> emaillist = new()
            {
                email
            };

            await SendMultipleEmail("Api Error Email", emaillist, err, null);
        }


        public async Task SendMultipleEmail(string subject, List<string> email, string content, List<string> attachments = null)
        {
            using var client = new SmtpClient(_configuration["EmailConfiguration:SmtpServer"], int.Parse(_configuration["EmailConfiguration:Port"]))
            {
                Credentials = new NetworkCredential(_configuration["EmailConfiguration:Username"], _configuration["EmailConfiguration:Password"]),
                EnableSsl = true
            };
            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_configuration["EmailConfiguration:From"]);
            for (int i = 0; i < email.Count; i++)
            {
                mailMessage.To.Insert(i, new MailAddress(email[i]));
            }
            mailMessage.Subject = subject;
            mailMessage.Body = content;
            mailMessage.IsBodyHtml = true;
            attachments?.ForEach(attachment =>
            {
                mailMessage.Attachments.Add(new Attachment(attachment));
            });
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            await client.SendMailAsync(mailMessage);
        }

        public async Task SendMultipleEmail(string subject, string email, string content, List<string> attachments = null)
        {
            try
            {
                using var client = new SmtpClient(_configuration["EmailConfiguration:SmtpServer"], int.Parse(_configuration["EmailConfiguration:Port"]))
                {
                    Credentials = new NetworkCredential(_configuration["EmailConfiguration:Username"], _configuration["EmailConfiguration:Password"]),
                    EnableSsl = true
                };
                using var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_configuration["EmailConfiguration:From"]);

                mailMessage.To.Insert(0, new MailAddress(email));

                mailMessage.Subject = subject;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;
                attachments?.ForEach(attachment =>
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                });
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                await client.SendMailAsync(mailMessage);
            }

            catch (Exception ex)
            {
                var wr = new StreamWriter($"{Directory.GetCurrentDirectory()}/log.txt", true, System.Text.Encoding.UTF8);
                wr.WriteLine(ex);
                wr.Close();
            }

        }

        //public async Task SendMail(string subject, string email, string content, List<string> attachments = null)
        //{


        //    string[] scopes = { GmailService.Scope.GmailSend };
        //    UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        new ClientSecrets
        //        {
        //            ClientId = "892814681891-ie10n677rhuqikd88fgn8lo1pri3rfke.apps.googleusercontent.com",
        //            ClientSecret = "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"
        //        },
        //        scopes,
        //        "user",
        //        CancellationToken.None
        //    ).Result;

        //    GmailService service = new GmailService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "Accounts"
        //    });

        //    var message = new Google.Apis.Gmail.v1.Data.Message();
        //    string messageStr = $"From: admin@beyondacademics.com\r\n" +
        //                        $"To: {email}\r\n" +
        //                        $"Subject:{subject}\r\n" +
        //                        "Content-Type: text/html; charset=UTF-8\r\n\r\n" +
        //                        content;
        //    message.Raw = Base64UrlEncode(messageStr);
        //    service.Users.Messages.Send(message, "me").Execute();
        //    Console.WriteLine("Email sent");

        //}

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private static string CreateEmail(string from, string to, string subject, string body)
        {
            string str = "From: " + from + "\r\n";
            str += "To: " + to + "\r\n";
            str += "Subject: " + subject + "\r\n";
            str += "\r\n" + body;
            return str;
        }
    }
}

