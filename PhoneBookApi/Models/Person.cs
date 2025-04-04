using System.ComponentModel.DataAnnotations;

namespace PhoneBookApi.Models
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        
        public Person() {
            Id = Guid.NewGuid();
            FirstName= string.Empty;
            LastName= string.Empty;
            Company = string.Empty; 
        }
    }

    

 
}
