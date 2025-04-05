using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Models.Dtos;

namespace PhoneBookApi.Services
{
    public class PhoneBookService
    {
        private readonly AppDbContext _context;

        public PhoneBookService(AppDbContext context)
        {
            _context = context;
        }

        public string CheckCreatePerson(PersonDto personDto)
        {
            string error=string.Empty;
            if (personDto == null) { return " Kullanıcı bilgileri boş olamaz."; }
            if (string.IsNullOrEmpty(personDto.FirstName)) { return " Kullanıcı adı boş olamaz."; }
            if (string.IsNullOrEmpty(personDto.LastName)) { return " Kullanıcı soyadı boş olamaz."; }
            if (string.IsNullOrEmpty(personDto.Company)) { return " Firma boş olamaz."; }
            return error;

        }
        public async Task<string> CreatePersonAsync(PersonDto personDto) 
        {
            string sonuc=string.Empty;
            Person newPerson = new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                Company = personDto.Company,
            };
            try
            {
                _context.PB_PERSON.Add(newPerson);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                sonuc= ex.Message;
            }
            
            return sonuc;
        }

        public async Task<List<Person>> ListPersonsAsync()
        {
             var persons = await _context.PB_PERSON.ToListAsync();
            return persons;
        }
    }
}
