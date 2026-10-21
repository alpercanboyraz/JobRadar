using JobRadar.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobRadar.Infrastructure.Persistence.Configurations;

internal sealed class JobAnalysisConfiguration : IEntityTypeConfiguration<JobAnalysis>
{
    public void Configure(EntityTypeBuilder<JobAnalysis> b)
    {
        b.HasKey(x => x.Id);

        b.HasOne<JobPosting>()
            .WithMany()
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.ContentHash).HasMaxLength(64).IsRequired();
        b.Property(x => x.Model).HasMaxLength(100).IsRequired();
        b.Property(x => x.Seniority).HasConversion<string>().HasMaxLength(16);
        b.Property(x => x.WorkMode).HasConversion<string>().HasMaxLength(16);

        b.HasIndex(x => new { x.JobPostingId, x.ContentHash });
    }
}
