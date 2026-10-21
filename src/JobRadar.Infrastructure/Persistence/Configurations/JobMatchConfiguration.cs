using JobRadar.Domain.Jobs;
using JobRadar.Domain.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobRadar.Infrastructure.Persistence.Configurations;

internal sealed class JobMatchConfiguration : IEntityTypeConfiguration<JobMatch>
{
    public void Configure(EntityTypeBuilder<JobMatch> b)
    {
        b.HasKey(x => x.Id);

        b.HasOne<JobPosting>()
            .WithMany()
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.CvVersion).HasMaxLength(64).IsRequired();
        b.Property(x => x.Model).HasMaxLength(100).IsRequired();
        b.Property(x => x.Reasoning).IsRequired();

        b.HasIndex(x => new { x.JobPostingId, x.CvVersion, x.Model });
        b.HasIndex(x => x.Score);
    }
}
