using DEPLOY.MongoBDEFCore.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace DEPLOY.MongoBDEFCore.API
{
    public class BoatEntityConfiguration : IEntityTypeConfiguration<Boat>
    {
        public void Configure(EntityTypeBuilder<Boat> builder)
        {
            builder.ToCollection("Boat");

            //builder.HasKey(e => e.Id);
            //builder.Property(e => e.Name).IsRequired();
            //builder.Property(e => e.size).IsRequired();
            //builder.Property(e => e.license).IsRequired();
            //builder.Property(e => e.Version).IsConcurrencyToken();
        }
    }
}
