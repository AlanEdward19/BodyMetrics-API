using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.AthleteGroups;
using BodyMetricsApi.Features.Sports;
using Microsoft.EntityFrameworkCore;

namespace BodyMetricsApi.Infrastructure.Persistence;

public sealed class BodyMetricsDbContext(DbContextOptions<BodyMetricsDbContext> options) : DbContext(options)
{
    public DbSet<Sport> Sports => Set<Sport>();

    public DbSet<Athlete> Athletes => Set<Athlete>();

    public DbSet<AthleteGroup> AthleteGroups => Set<AthleteGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sport>().HasKey(sport => sport.Id);
        modelBuilder.Entity<Athlete>().HasKey(athlete => athlete.Id);
        modelBuilder.Entity<AthleteGroup>().HasKey(group => group.Id);

        base.OnModelCreating(modelBuilder);
    }
}

