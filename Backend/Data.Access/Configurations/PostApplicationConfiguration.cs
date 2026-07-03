using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PostApplicationConfiguration : IEntityTypeConfiguration<PostApplication>
{
    public void Configure(EntityTypeBuilder<PostApplication> builder)
    {
        builder.HasOne(x => x.User)
            .WithMany(x => x.PostApplications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}