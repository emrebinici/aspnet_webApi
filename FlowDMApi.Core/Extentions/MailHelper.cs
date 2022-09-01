using System.Collections.Generic;
using System.Net.Mail;

namespace FlowDMApi.Core.Extentions
{
   public class MailHelper
    {
        private string Host { get; set; }
        private int Port { get; set; }
        private string KullaniciSifre { get; set; }
        private string GonderenMail { get; set; }
        private string MailImza { get; set; }
        private bool SSL { get; set; }

        public MailHelper(string Host, int Port,  string Sifre, string GonderenMail, string MailImza, bool SSL)
        {
            this.Host = Host;
            this.Port = Port;
            this.KullaniciSifre = Sifre;
            this.GonderenMail = GonderenMail;
            this.MailImza = MailImza;
            this.SSL = SSL;
        }


        public void Gonder(List<string> adresler, string MailKonu, string MailIcerik)
        {
            SmtpClient mail = new SmtpClient();
            mail.Host = Host;
            mail.Port = Port;
            mail.Credentials = new System.Net.NetworkCredential(GonderenMail, KullaniciSifre);
            mail.EnableSsl = SSL;
            MailMessage mesaj = new MailMessage();
            mesaj.IsBodyHtml = true;
            mesaj.Priority = MailPriority.High;
            mesaj.From = new MailAddress(GonderenMail);
            foreach (string adres in adresler)
            {
                mesaj.To.Add(adres);
            }
            mesaj.Subject = MailKonu;
            mesaj.Body = MailIcerik + "<br><br><br>" + MailImza;
            mail.Send(mesaj);
        }

        public void Gonder(List<string> adresler, string MailKonu, string MailIcerik, List<Attachment> dosyalar)
        {
            SmtpClient mail = new SmtpClient();
            mail.Host = Host;
            mail.Port = Port;
            mail.Credentials = new System.Net.NetworkCredential(GonderenMail, KullaniciSifre);
            mail.EnableSsl = SSL;
            MailMessage mesaj = new MailMessage();
            mesaj.IsBodyHtml = true;
            mesaj.Priority = MailPriority.High;
            mesaj.From = new MailAddress(GonderenMail);
            foreach (string adres in adresler)
            {
                mesaj.To.Add(adres);
            }
            foreach (Attachment dosya in dosyalar)
            {
                mesaj.Attachments.Add(dosya);
            }
            mesaj.Subject = MailKonu;
            mesaj.Body = MailIcerik + "<br><br><br>" + MailImza;
            mail.Send(mesaj);
        }
    }
}
