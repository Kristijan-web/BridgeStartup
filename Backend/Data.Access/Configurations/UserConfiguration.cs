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

            // Treba da kazem da kada se brise user da se to ne dozvoli ako postoji userid u tabeli PostApplications?
            // - Da bih to uspeo moram li da pravim i konfiguraciju za PostApplications?

            // Kontam da pre mora da se ode u PostAPpliucations, jer je tu foreign key


        }
    }
}
