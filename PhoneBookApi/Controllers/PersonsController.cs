using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Models.Dtos;
using PhoneBookApi.Services;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhoneBookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonsController(AppDbContext context)
        {
            _context = context;
        }
  
        [HttpGet("PersonsList")]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await new PhoneBookService(_context).ListPersonsAsync();
            return Ok(persons);
        }

        [HttpPost("PostPerson")]
        public async Task<IActionResult> PostPerson(PersonDto personDto)
        {
            string hata= new PhoneBookService(_context).CheckCreatePerson(personDto);
            if (!string.IsNullOrEmpty(hata)) {return BadRequest(hata); }

            string sonuc = await new PhoneBookService(_context).CreatePersonAsync(personDto);
            if (string.IsNullOrEmpty(sonuc)) { return Ok(); }
            else { return BadRequest(sonuc); }
        }

       

        [HttpDelete("DeletePerson/{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            var person = await _context.PB_PERSON.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            var personContact = await _context.PB_CONTACT_INFO.Where(x => x.PersonId == id).ToListAsync();
            if (personContact.Count>0)
            {
                _context.PB_CONTACT_INFO.RemoveRange(personContact);
            }

            _context.PB_PERSON.Remove(person);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("PostContractInfo")]
        public async Task<IActionResult> PostContractInfo(ContactInfoDto contactInfo)
        {
            if (contactInfo == null)
            {
                return NotFound();
            }

            ContactInfo newContractInfo = new ContactInfo()
            {
                Id = Guid.NewGuid(),
                PersonId = contactInfo.PersonId,
                ContactType = contactInfo.ContactType,
                Info = contactInfo.Info,
            };



            _context.PB_CONTACT_INFO.Add(newContractInfo);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("DeleteContactInfo/{id}")]
        public async Task<IActionResult> DeleteContactInfo(Guid id)
        {
            var contactInfo = await _context.PB_CONTACT_INFO.FindAsync(id);
            if (contactInfo == null)
            {
                return NotFound();
            }

            _context.PB_CONTACT_INFO.Remove(contactInfo);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("PersonsDetailList")]
        public async Task<IActionResult> GetAllPersonsDetails()
        {
            List<PersonInfo> liste = new();
            var persons = await _context.PB_PERSON.ToListAsync();

            foreach (var person in persons)
            {
                PersonInfo pi = new PersonInfo();
                pi.Person=person;
                var contacts = await _context.PB_CONTACT_INFO.Where(x=>x.PersonId==person.Id).ToListAsync();
                if (contacts.Count>0)
                {
                  pi.ContactInfos = contacts;
                }
                else
                {
                    pi.ContactInfos = new List<ContactInfo>();
                }
                liste.Add(pi);
               
            }

            return Ok(liste);
        }
    }
}
