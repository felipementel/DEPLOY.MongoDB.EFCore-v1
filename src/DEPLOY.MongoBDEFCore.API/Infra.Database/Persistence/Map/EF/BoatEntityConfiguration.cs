using DEPLOY.MongoBDEFCore.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;

namespace DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence.Map.EF
{
    public class BoatEntityConfiguration : IEntityTypeConfiguration<Boat>
    {
        public void Configure(EntityTypeBuilder<Boat> builder)
        {
            builder.ToCollection("Boat");

            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasBsonRepresentation(BsonType.ObjectId).HasValueGenerator<ObjectIdGenerator>();
        }
    }

    public class ObjectIdGenerator : ValueGenerator<ObjectId>
    {
        public override ObjectId Next(EntityEntry entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }

            return ObjectId.GenerateNewId();
        }
        public override bool GeneratesTemporaryValues => false;
    }
}
