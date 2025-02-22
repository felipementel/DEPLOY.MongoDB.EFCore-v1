using DEPLOY.MongoBDEFCore.API.Domain;
using DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence.Map.EF;
using Microsoft.EntityFrameworkCore;

namespace DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence
{
    public class MongoDBContext : DbContext
    {
        public MongoDBContext(DbContextOptions<MongoDBContext> options) : base(options)
        {
        }

        public DbSet<Boat> Boats { get; set; }
        public DbSet<Marina> Marinas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BoatEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MarinaEntityConfiguration());
        }
    }
}
