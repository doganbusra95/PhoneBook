using System.ComponentModel.DataAnnotations;

namespace PhoneBookApi.Models.Dtos
{
    public class ContactInfoDto
    {
    
        public Guid PersonId { get; set; }
        public ContactType ContactType { get; set; }
        public string Info { get; set; }

    }

}
