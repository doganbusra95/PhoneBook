using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Models.Dtos;
using System.Dynamic;
using System.Data;
using PhoneBookSharedTools.ExcelFileWorks;
using PhoneBookApi.Settings.Mail;

namespace PhoneBookApi.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reports>> GetAllReportsAsync()
        {
            List<Reports> reports = new List<Reports>();
            reports = await _context.PB_REPORTS.ToListAsync();
            return reports;

        }

        public async Task<ReportDto> CreateReportAsync()
        {
            ReportDto reportDto = new ReportDto();
            var report = new Reports
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.Now.ToUniversalTime(),
                Status = ReportStatus.Preparing,
            };


            try
            {
                _context.PB_REPORTS.Add(report);
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {

                reportDto.ErrorMessage = ex.Message;
            }

            if (string.IsNullOrEmpty(reportDto.ErrorMessage)) { reportDto.Id = report.Id; }
            return reportDto;
        }
        public async Task GenerateReport(Guid reportId)
        {
            var report = await _context.PB_REPORTS.FindAsync(reportId);

            if (report == null) return;

            try
            {
                byte[] bytes = new byte[0];
                var locationStats = await _context.PB_CONTACT_INFO
                    .Where(x => x.ContactType == ContactType.Location)
                    .GroupBy(x => x.Info)
                    .Select(group => new LocationStat
                    {
                        Location = group.Key,
                        PersonCount = group.Select(x=>x.PersonId).Distinct().Count(),
                        PhoneNumberCount = _context.PB_CONTACT_INFO.Where(ci => group.Select(g => g.PersonId).Contains(ci.PersonId) && ci.ContactType == ContactType.Phone_Number).Count()
                    })
                    .ToListAsync();
                if (locationStats.Count>0)
                {
                    bytes=ExportToExcelForGetListLocationStats(locationStats);
                    Mail mail = new Mail
                    {
                        PostAddress = "doganbusra.95@outlook.com",
                        Subject="pentanom",
                        Body="pentanom",
                        File=bytes,
                        FileName= $"report_{DateTime.Now.ToString("ddMMyyyy")}.xlsx"
                    };
                    var sonuc=SendEmail(mail);
                    if (sonuc == "OK")
                    {
                        report.Status = ReportStatus.Completed;
                    }
                    else
                    {
                        report.Status = ReportStatus.Failed;
                    }


                }
                else
                {
                    report.Status = ReportStatus.Failed;
                }

          
                await _context.SaveChangesAsync();

           
            }
            catch (Exception ex)
            {
            
                report.Status = ReportStatus.Failed;
                await _context.SaveChangesAsync();
            }
        }

        public byte[] ExportToExcelForGetListLocationStats(List<LocationStat> liste)
        {
            using (DataSet dataSet = new())
            {
                DataTable dt1 = new("RAPOR");

                dt1.Columns.Add(new DataColumn() { Caption = "LOCATION", DataType = typeof(string) });
                dt1.Columns.Add(new DataColumn() { Caption = "PERSON COUNT", DataType = typeof(int) });
                dt1.Columns.Add(new DataColumn() { Caption = "PHONE NUMBER COUNT", DataType = typeof(int) });

                foreach (var item in liste)
                {
                    DataRow dr = dt1.NewRow();
                    dr.ItemArray = new object[]
                        {
                            item.Location,
                            item.PersonCount,
                            item.PhoneNumberCount

                         };
                    dt1.Rows.Add(dr);

                }

                dataSet.Tables.Add(dt1);

                IExcelFileWorks _excelIsleri = ServiceTool.ServiceProvider.GetService<IExcelFileWorks>();

                return _excelIsleri.ExcelDosyasiGetir(dataSet);

            }
        }


        public string SendEmail(Mail mail)
        {
            string sonuc = "OK";

            if (!string.IsNullOrEmpty(mail.PostAddress))
            {
                MailInfo pb = MailInfo(mail.PostAddress);

                MimeMessage eMail = new MimeMessage();
                eMail.From.Add(new MailboxAddress(pb.DisplayName, pb.MailAddress));
                eMail.Sender = new MailboxAddress(pb.DisplayName, pb.MailAddress);
                eMail.Subject = mail.Subject;
                eMail.Body = new TextPart(TextFormat.Html) { Text = mail.Body };
                string[] mailAdresleri = mail.PostAddress.Split(";".ToCharArray());
                foreach (string adres in mailAdresleri)
                {
                    if (!string.IsNullOrEmpty(adres)) eMail.Bcc.Add(MailboxAddress.Parse(adres));
                }
                if (!string.IsNullOrEmpty(mail.FileName) && mail.File.Length > 0)
                {
                    var builder = new BodyBuilder() { TextBody = mail.Body };
                    builder.Attachments.Add(mail.FileName, mail.File);
                    eMail.Body = builder.ToMessageBody();
                }
                MailKit.Net.Smtp.SmtpClient smtp = new MailKit.Net.Smtp.SmtpClient();
                try
                {
                    smtp.Connect(pb.MailServer, pb.Port, MailKit.Security.SecureSocketOptions.Auto);
                    smtp.Authenticate(pb.UserName, pb.Password);
                    smtp.Send(eMail);
                }
                catch (Exception h)
                {
                    sonuc = h.Message.ToString();
                }
                finally
                {
                    smtp.Disconnect(true);
                }
            }
            else
            {
                sonuc = "Mail Adresi Boş Olamaz.!";
            }
            return sonuc;
        }

        private MailInfo MailInfo(string MailAddresses)
        {
            string[] mailAddresses = MailAddresses.Split(";".ToCharArray());
            int addressNumber = mailAddresses.Where(x => !string.IsNullOrEmpty(x)).Count();

            if (EmailCounter.Counter + addressNumber < 30)
            {
                EmailCounter.Counter += addressNumber;
            }
            else
            {
                EmailCounter.Counter = addressNumber;
                EmailCounter.MailAccountOrder++;
                if (EmailCounter.MailAccountOrder >= 6)
                    EmailCounter.MailAccountOrder = 0;
            }

            return EmailCounter.pbs[EmailCounter.MailAccountOrder];
        }


    }

 
 
  


}
