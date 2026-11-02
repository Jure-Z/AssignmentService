using AssignmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}