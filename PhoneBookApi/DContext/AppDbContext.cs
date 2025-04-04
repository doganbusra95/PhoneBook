using Microsoft.EntityFrameworkCore;
using PhoneBookApi.Models;

namespace PhoneBookApi.DContext
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> PB_PERSON { get; set; }
        public DbSet<ContactInfo> PB_CONTACT_INFO { get; set; }

        public DbSet<Reports> PB_REPORTS { get; set; }
    }
}
