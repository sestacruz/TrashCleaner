using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TrashCleaner.Log;

namespace TrashCleaner.Mail
{
    public class SMTP
    {
        private static string _from = "";
        private static string _fromName = $"TrashCleaner {Environment.MachineName}";
        private static string _to = "";
        private static string _smtpUser = "";
        private static string _smtpPass = "";
        private static string _host = "smtp.fibertel.com.ar";

        public static void SendReport()
        {
            MailMessage reportMail = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(_from,_fromName),
                Subject = $"[TrashCleaner] Informe {Environment.MachineName} - {DateTime.Now:yyyyMMdd}",
            };
            reportMail.To.Add(new MailAddress(_to));

            Attachment img = new Attachment(@".\Mail\Template\bulldozer.png");
            img.ContentDisposition.Inline = true;

            reportMail.Body = Report.GetHtmlReport().Replace("{0}",img.ContentId);
            reportMail.Attachments.Add(new Attachment(Logger.name));
            reportMail.Attachments.Add(img);

            using(var client = new SmtpClient(_host))
            {
                try
                {
                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    client.EnableSsl = false;
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                                                                                        X509Certificate certificate,
                                                                                        X509Chain chain,
                                                                                        SslPolicyErrors sslPolicyErrors) { return true; };
                    client.Send(reportMail);
                    reportMail.Dispose();
                }
                catch (Exception e)
                {
                    reportMail.Dispose();
                    Logger.WriteLogFile("Error al enviar el informe por mail");
                    Logger.WriteLogFile(e.Message);
                }
            }
        }

        //private static GenerateTable()
    }
}
