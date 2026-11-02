using AssignmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssignmentService.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Content)
            .IsRequired();

        // Pretvorba enuma v niz (string) v bazi podatkov za lažje branje
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();

        // Relacija: Submission -> Assignment
        builder.HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacija: Submission -> Evaluation
        builder.HasOne(s => s.Evaluation)
            .WithOne(e => e.Submission)
            .HasForeignKey<Evaluation>(e => e.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
