using AssignmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssignmentService.Infrastructure.Persistence.Configurations;

public class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
{
    public void Configure(EntityTypeBuilder<Evaluation> builder)
    {
        builder.ToTable("Evaluations");

        builder.HasKey(e => e.Id);

        // Definiramo natančnost decimalnega števila za ocene
        builder.Property(e => e.Score)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(e => e.Feedback)
            .HasMaxLength(2000);

        builder.Property(e => e.EvaluatedAt)
            .IsRequired();
    }
}
