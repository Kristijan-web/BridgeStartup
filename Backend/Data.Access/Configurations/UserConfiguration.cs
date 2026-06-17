using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Access.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        //  Sta ovaj interfejs zahteva?
        // - Da definisemo metodu


        // Interfejs nas forsira da definisemo logiku za Configure metodu,
        // interfejs je definisao i ulazne parametre



        public void Configure(EntityTypeBuilder<User> builder)
        {

            // Trebam da stavim defaultnu vrednost na kolonu Role?
            // Kako se to radi?

            builder.Property(x => x.RoleId).HasDefaultValue(1);
            // Email mora da bude unique
            // Kako se to radi?
            // - Dodaje se unique index

            builder.HasIndex(x => x.Email).IsUnique();






        }
    }
}
