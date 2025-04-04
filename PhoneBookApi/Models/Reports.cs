using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PhoneBookApi.Models
{
    public class Reports
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime RequestDate { get; set; }
        public ReportStatus Status { get; set; }

        
        public Reports() {
            Id = Guid.NewGuid();
            RequestDate= DateTime.Now;
            Status= ReportStatus.Preparing;
            
        }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReportStatus
    {
        Preparing = 10, Completed = 100 , Failed=110
    }




}
