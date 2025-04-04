using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PhoneBookApi.Models
{
    public class ContactInfo
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public ContactType ContactType { get; set; }
        public string Info { get; set; }

    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContactType
    {
        Phone_Number=10,Email=20,Location=30
    }
}
