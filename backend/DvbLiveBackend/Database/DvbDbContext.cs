using DerMistkaefer.DvbLive.Backend.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace DerMistkaefer.DvbLive.Backend.Database
{
    public sealed class DvbDbContext : DbContext
    {
        public DbSet<StopPoints> StopPoints { get; set; }


        public DvbDbContext(DbContextOptions options) : base(options) { }

    }
}
