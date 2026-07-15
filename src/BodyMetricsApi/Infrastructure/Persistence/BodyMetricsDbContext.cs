using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.Sports;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Infrastructure.Persistence;

// AthleteGroup is deliberately not part of this EF model: it embeds Athlete (itself an
// EF entity with its own DbSet), and EF's relational-style change tracking can't cleanly
// own that graph for a document store. It's persisted directly via MongoDbContext instead.
public sealed class BodyMetricsDbContext(DbContextOptions<BodyMetricsDbContext> options) : DbContext(options)
{
    public DbSet<Sport> Sports => Set<Sport>();

    public DbSet<Athlete> Athletes => Set<Athlete>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sport>().HasKey(sport => sport.Id);
        modelBuilder.Entity<Athlete>().HasKey(athlete => athlete.Id);

        base.OnModelCreating(modelBuilder);
    }
}

