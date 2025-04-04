namespace PhoneBookApi.Models.Dtos
{
    public class PersonDto
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }

        public PersonDto()
        {

            FirstName = string.Empty;
            LastName = string.Empty;
            Company = string.Empty;
        }
    }
}
