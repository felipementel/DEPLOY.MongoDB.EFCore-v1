using DEPLOY.MongoBDEFCore.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace DEPLOY.MongoBDEFCore.API.Infra.Database.Persistence.Map.EF
{
    public class MarinaEntityConfiguration : IEntityTypeConfiguration<Marina>
    {
        public void Configure(EntityTypeBuilder<Marina> builder)
        {
            builder.ToCollection("Marina");
        }
    }
}
