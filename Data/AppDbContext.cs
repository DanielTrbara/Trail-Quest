using Microsoft.EntityFrameworkCore;
using StepUp.Models;

namespace StepUp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Scan> Scans => Set<Scan>();
    }
}