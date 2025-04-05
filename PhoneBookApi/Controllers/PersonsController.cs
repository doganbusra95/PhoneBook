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
        private readonly PhoneBookService _phoneBookService;
        public PersonsController(AppDbContext context)
        {
            _context = context;
            _phoneBookService = new PhoneBookService(_context);
        }
  
        [HttpGet("PersonsList")]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _phoneBookService.ListPersonsAsync();
            return Ok(persons);
        }

        [HttpPost("PostPerson")]
        public async Task<IActionResult> PostPerson(PersonDto personDto)
        {
            string hata= _phoneBookService.CheckCreatePerson(personDto);
            if (!string.IsNullOrEmpty(hata)) {return BadRequest(hata); }

            string sonuc = await _phoneBookService.CreatePersonAsync(personDto);
            if (string.IsNullOrEmpty(sonuc)) { return Ok(); }
            else { return BadRequest(sonuc); }
        }

       

        [HttpDelete("DeletePerson/{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            string sonuc = await _phoneBookService.DeletePersonAndAllContactsAsync(id);
            if (string.IsNullOrEmpty(sonuc)) { return Ok(); }
            else { return BadRequest(sonuc); }
        }


        [HttpPost("PostContractInfo")]
        public async Task<IActionResult> PostContractInfo(ContactInfoDto contactInfoDto)
        {

            string hata = _phoneBookService.CheckCreateContactInfo(contactInfoDto);
            if (!string.IsNullOrEmpty(hata)) { return BadRequest(hata); }

            string sonuc = await _phoneBookService.CreateContactInfoAsync(contactInfoDto);
            if (string.IsNullOrEmpty(sonuc)) { return Ok(); }
            else { return BadRequest(sonuc); }
            
        }


        [HttpDelete("DeleteContactInfo/{id}")]
        public async Task<IActionResult> DeleteContactInfo(Guid id)
        {
            string sonuc = await _phoneBookService.DeleteContactInfoAsync(id);
            if (string.IsNullOrEmpty(sonuc)) { return Ok(); }
            else { return BadRequest(sonuc); }

           
        }

        [HttpGet("PersonsDetailList")]
        public async Task<IActionResult> GetAllPersonsDetails()
        {
            List<PersonInfo> liste = new();
            liste=await _phoneBookService.ListPersonInfosAsync();

            return Ok(liste);
        }
    }
}
