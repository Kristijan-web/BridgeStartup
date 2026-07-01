using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Access.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {



            builder.Property(x => x.RoleId).HasDefaultValue(1);


            builder.HasIndex(x => x.Email).IsUnique();


        }
    }
}
