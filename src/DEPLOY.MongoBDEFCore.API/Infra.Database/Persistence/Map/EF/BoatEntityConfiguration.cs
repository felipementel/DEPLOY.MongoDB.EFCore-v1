using DEPLOY.MongoBDEFCore.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.EntityFrameworkCore.ValueGeneration;

namespace DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence.Map.EF
{
    public class BoatEntityConfiguration : IEntityTypeConfiguration<Boat>
    {
        public void Configure(EntityTypeBuilder<Boat> builder)
        {
            builder.ToCollection("Boat");

            builder
            .HasKey(x => x.Id);

            builder
            .Property(x => x.Id)
            .HasConversion<MongoDB.Bson.ObjectId>()
            .HasValueGenerator<StringObjectIdValueGenerator>();
        }
    }
}
