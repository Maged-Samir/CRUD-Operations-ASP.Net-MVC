using Microsoft.EntityFrameworkCore;

namespace Squad.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Club> Clubs { get; set; }
    }
}
