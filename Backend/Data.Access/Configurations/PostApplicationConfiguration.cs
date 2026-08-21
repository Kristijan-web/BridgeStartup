using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PostApplicationConfiguration : IEntityTypeConfiguration<PostApplication>
{
    public void Configure(EntityTypeBuilder<PostApplication> builder)
    {

        // Zbog cega postoji kod ispod?
        // - Kada se vrsi cascadno brisanje i ako se do nekog zapisa u x tabeli moze dodji preko vise od 1 tabele onda nastaje greska.

        // Primer u ovom contextu
        // - Ako se obrise zapis u tabeli User sa ukljucenim cascandim brisanjem onda se do zapisa u PostApplication moze dodji iz tabele User->PostApplication i User->Post->PostApplication

        // Kako se naziva ova greska
        // - Ovo sql server zove "multiple cascade paths"

        // Sta radi nacin brisanja "NoAction"?
        // - Ako pokusa da se obrise parent zapis kojeg referenciraju child zapisi onda se brisanje nece dozvoliti, sto znaci da prvo ce morati ti child zapisi da se obrisu

        // Koji nacini brisanja u SQL-u postoje?
        // - Cascade -> kada se obrise parent brisu se i svi child-ovi
        // - SET NULL 
        // - No Action
        // - Set default

        builder.HasOne(x => x.User)
            .WithMany(x => x.PostApplications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasKey(x => new { x.UserId, x.PostId });
    }
}