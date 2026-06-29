using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Access.Configurations
{
    public class RoleUseCasesConfiguration : IEntityTypeConfiguration<RoleUseCases>
    {
        public void Configure(EntityTypeBuilder<RoleUseCases> builder)
        {
            // ovde sad konfiugurisem

            builder.HasKey(x => new { x.RoleId, x.UseCasesId });
        }
    }
}
