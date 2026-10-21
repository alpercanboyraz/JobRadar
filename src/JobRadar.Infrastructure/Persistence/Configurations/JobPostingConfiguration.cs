using JobRadar.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobRadar.Infrastructure.Persistence.Configurations;

internal sealed class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Source).HasConversion<string>().HasMaxLength(32);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);

        b.Property(x => x.ExternalId).HasMaxLength(128).IsRequired();
        b.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
        b.Property(x => x.Title).HasMaxLength(500).IsRequired();
        b.Property(x => x.Location).HasMaxLength(300);
        b.Property(x => x.Url).HasMaxLength(2000).IsRequired();
        b.Property(x => x.Description).IsRequired();
        b.Property(x => x.ContentHash).HasMaxLength(64).IsRequired();

        b.HasIndex(x => new { x.Source, x.ExternalId }).IsUnique();
        b.HasIndex(x => new { x.Source, x.CompanyName });
    }
}
