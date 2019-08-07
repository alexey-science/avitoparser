using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AvitoInformer
{
    class service
    {
        public static string getDate(string nfDate) 
        {
            string[] datearray = nfDate.Split(' ');
            string fDate = "";
            DateTime date;
            string[] hm;
            if (datearray.Length == 2)
            {
                switch (datearray[0])
                {
                    case "Сегодня":
                        date = DateTime.Today;
                        hm = datearray[1].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", date.Day, date.Month, date.Year, hm[0], hm[1]);
                        break;
                    case "Вчера":
                        date = DateTime.Today;
                        hm = datearray[1].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", date.Day - 1, date.Month,  date.Year, hm[0], hm[1]);
                        break;
                    default:
                        fDate = "none";
                        break;
                }
            }
            if(datearray.Length == 3)
            {
                string month = datearray[1].Substring(0, 3);
                switch (month)
                {
                    case "янв":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 01, date.Year, hm[0], hm[1]);
                        break;
                    case "фев":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 02,  date.Year, hm[0], hm[1]);
                        break;
                    case "мар":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 03, date.Year, hm[0], hm[1]);
                        break;
                    case "апр":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 04,  date.Year, hm[0], hm[1]);
                        break;
                    case "мая":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 05,  date.Year, hm[0], hm[1]);
                        break;
                    case "июн":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 06,  date.Year, hm[0], hm[1]);
                        break;
                    case "июл":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 07,  date.Year, hm[0], hm[1]);
                        break;
                    case "авг":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 08,  date.Year, hm[0], hm[1]);
                        break;
                    case "сен":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 09,  date.Year, hm[0], hm[1]);
                        break;
                    case "окт":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 10,  date.Year, hm[0], hm[1]);
                        break;
                    case "ноя":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 11,  date.Year, hm[0], hm[1]);
                        break;
                    case "дек":
                        date = DateTime.Today;
                        hm = datearray[2].Split(':');
                        fDate = string.Format("{0}/{1}/{2} {3}:{4}:00", datearray[0], 12,  date.Year, hm[0], hm[1]);
                        break;
                    default:
                        fDate = "none";
                        break;
                }
            }
            return fDate;
        
        }

        public static void SendMail(string email, string caption, string body, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(//email);
                mail.To.Add(new MailAddress(email));
                mail.Subject = caption;
                mail.Body = body;
                if (!string.IsNullOrEmpty(attachFile))
                    mail.Attachments.Add(new Attachment(attachFile));
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(//email anp pass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }

        }
    }
}
