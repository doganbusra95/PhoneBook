using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Models.Dtos;
using System;
using System.Security.Cryptography.Pkcs;

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

        public async Task<List<ContactInfo>> ListContactInfosAsync(Guid personId)
        {
            var contactInfosById = await _context.PB_CONTACT_INFO.Where(x => x.PersonId == personId).ToListAsync();
            return contactInfosById;
        }

        public async Task<string> DeletePersonAndAllContactsAsync(Guid id)
        {
            string hata = string.Empty;
            var person = await _context.PB_PERSON.FindAsync(id);
            if (person == null)
            {
                return "Kullanıcı bulunamadı.";
            }

            var personContacts = await ListContactInfosAsync(person.Id);
            
            try
            {
                if (personContacts.Count > 0)
                {
                    _context.PB_CONTACT_INFO.RemoveRange(personContacts);
                }

                _context.PB_PERSON.Remove(person);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                hata = ex.Message;
            }

            return hata;
        }

        public string CheckCreateContactInfo(ContactInfoDto contactInfoDto)
        {
            string error = string.Empty;
            if (contactInfoDto == null) { return " Kullanıcı iletişim bilgileri boş olamaz."; }
            if (string.IsNullOrEmpty(contactInfoDto.Info)) { return " Kullanıcı iletişim bilgisi boş olamaz."; }
           
            return error;

        }
        public async Task<string> CreateContactInfoAsync (ContactInfoDto contactInfoDto)
        {
            string sonuc = string.Empty;
          

            ContactInfo newContractInfo = new ContactInfo()
            {
                Id = Guid.NewGuid(),
                PersonId = contactInfoDto.PersonId,
                ContactType = contactInfoDto.ContactType,
                Info = contactInfoDto.Info,
            };

            try
            {
                _context.PB_CONTACT_INFO.Add(newContractInfo);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                sonuc = ex.Message;
            }

            return sonuc;
        }

        public async Task<string> DeleteContactInfoAsync(Guid id)
        {
            string hata = string.Empty;

            var contactInfo = await _context.PB_CONTACT_INFO.FindAsync(id);
            if (contactInfo == null)
            {
                return "iletişim bilgisi bulunamadı.";
            }

            try
            {
                _context.PB_CONTACT_INFO.Remove(contactInfo);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                hata = ex.Message;
            }

            return hata;
        }

        public async Task<List<PersonInfo>> ListPersonInfosAsync()
        {
            List<PersonInfo> liste = new();
            var persons = await ListPersonsAsync();

            foreach (var person in persons)
            {
                PersonInfo pi = new PersonInfo();
                pi.Person = person;
                var contacts = await ListContactInfosAsync(person.Id);
                if (contacts.Count > 0)
                {
                    pi.ContactInfos = contacts;
                }
                else
                {
                    pi.ContactInfos = new List<ContactInfo>();
                }
                liste.Add(pi);

            }
            return liste;
        }
    }
}
