using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StargateAPI.Business.Data;

public class StargateContext(DbContextOptions<StargateContext> options) 
    : DbContext(options)
{
    public IDbConnection Connection => Database.GetDbConnection();
    public DbSet<Person> People { get; set; }
    public DbSet<AstronautDetail> AstronautDetails { get; set; }
    public DbSet<AstronautDuty> AstronautDuties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StargateContext).Assembly);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            SeedData(modelBuilder);
        }

        base.OnModelCreating(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        #pragma warning disable 0618 
        //add seed data
        modelBuilder.Entity<Person>()
            .HasData(
                new Person(
                    id: 1,
                    name: "John Doe"),
                new Person(
                    id: 2,
                    name: "Jane Doe"));

        modelBuilder.Entity<AstronautDetail>()
            .HasData(
                new AstronautDetail(
                    id: 1,
                    personId: 1,
                    currentRank: "1LT",
                    currentDutyTitle: "Commander",
                    careerStartDate: DateTime.Now,
                    careerEndDate: null));

        modelBuilder.Entity<AstronautDuty>()
            .HasData(
                new AstronautDuty(
                    id: 1,
                    personId: 1,
                    dutyStartDate: DateTime.Now,
                    dutyTitle: "Commander",
                    rank: "1LT",
                    dutyEndDate: null));
        #pragma warning restore 0618
    }
}
