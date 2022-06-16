using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ReProServices.Infrastructure.Smtp
{
    public class EmailHelper
    {
        private string _host;
        private string _from;
        private string _alias;
        private string _password;
        private int _port;
        public EmailHelper(IConfiguration iConfiguration)
        {
            var smtpSection = iConfiguration.GetSection("SMTP");
            if (smtpSection != null)
            {
                _host = smtpSection.GetSection("Host").Value;
                _from = smtpSection.GetSection("From").Value;
                _alias = smtpSection.GetSection("Alias").Value;
                _password = smtpSection.GetSection("Password").Value;
                _port = Convert.ToInt32(smtpSection.GetSection("port").Value);
            }
        }

        public bool SendEmail(EmailModel emailModel)
        {
            try
            {
                // using (SmtpClient client = new SmtpClient(_host,8889))
                using (SmtpClient client = new SmtpClient(_host, _port))
                {
                    NetworkCredential NetCrd = new NetworkCredential(_from, _password);
                    MailMessage mailMessage = new MailMessage();
                    // mailMessage.From = new MailAddress(_from, _alias);

                    //var imageResource = new LinkedResource(new MemoryStream(emailModel.FileBlob), "image/png") { ContentId = "added-image-id" };
                    //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(emailModel.Message, null, "text/html");
                    //htmlView.LinkedResources.Add(imageResource);
                    //mailMessage.AlternateViews.Add(htmlView);
                    mailMessage.From = new MailAddress(_from, _alias);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.To.Add(emailModel.To);
                    mailMessage.Body = emailModel.Message;
                    mailMessage.Subject = emailModel.Subject;
                    mailMessage.IsBodyHtml = emailModel.IsBodyHtml;

                    if (emailModel.attachments != null)
                    {
                        foreach (var attachment in emailModel.attachments)
                        {
                            MemoryStream stream = new MemoryStream(attachment.MemoryStream);
                            mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.FileType));
                        }
                    }
                    client.UseDefaultCredentials = false;
                    client.Credentials = NetCrd;
                    client.EnableSsl = true; // Node :this should be enabled for live service
                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception e)
            {
                //throw; //todo log error message
                //return false;
                throw new ApplicationException(e.Message);
            }
        }

        public bool SendEmail(EmailModel emailModel, LinkedResource res)
        {
            try
            {
               //  using (SmtpClient client = new SmtpClient(_host,8889))
                using (SmtpClient client = new SmtpClient(_host, _port))
                {
                    NetworkCredential NetCrd = new NetworkCredential(_from, _password);
                    MailMessage mailMessage = new MailMessage();
                    // mailMessage.From = new MailAddress(_from, _alias);

                    //var imageResource = new LinkedResource(new MemoryStream(emailModel.FileBlob), "image/png") { ContentId = "added-image-id" };
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(emailModel.Message, null, "text/html");
                    htmlView.LinkedResources.Add(res);
                    mailMessage.AlternateViews.Add(htmlView);

                    mailMessage.From = new MailAddress(_from, _alias);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.To.Add(emailModel.To);
                  
                    if (!string.IsNullOrEmpty(emailModel.CC))
                    {
                        var cclist = emailModel.CC.Split(',');
                        foreach (var cc in cclist)
                        {
                            if (!string.IsNullOrEmpty(cc))
                            {
                                MailAddress copy = new MailAddress(cc);
                                mailMessage.CC.Add(copy);
                            }
                        }
                    }
                    mailMessage.Subject = emailModel.Subject;
                    mailMessage.IsBodyHtml = emailModel.IsBodyHtml;

                    if (emailModel.attachments != null)
                    {
                        foreach (var attachment in emailModel.attachments)
                        {
                            MemoryStream stream = new MemoryStream(attachment.MemoryStream);
                            mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.FileType));
                        }
                    }
                    client.UseDefaultCredentials = false;
                    client.Credentials = NetCrd;
                    client.EnableSsl = true; // Node :this should be enabled for live service
                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception e)
            {
                //throw; //todo log error message
                //return false;
                throw new ApplicationException(e.Message);
            }
        }

    }
}
