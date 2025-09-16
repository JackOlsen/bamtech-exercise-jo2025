using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("AstronautDetail")]
public class AstronautDetail
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string CurrentRank { get; set; } = string.Empty;
    public string CurrentDutyTitle { get; set; } = string.Empty;
    public DateTime CareerStartDate { get; set; }
    public DateTime? CareerEndDate { get; set; }
    public virtual Person Person { get; set; } = null!;

    [Obsolete("For EF use only", error: true)]
    protected AstronautDetail() { }

    public AstronautDetail(
        string currentRank,
        string currentDutyTitle,
        DateTime careerStartDate,
        DateTime? careerEndDate)
        : this(
              id: default,
              personId: default,
              currentRank: currentRank,
              currentDutyTitle: currentDutyTitle,
              careerStartDate: careerStartDate,
              careerEndDate: careerEndDate)
    {
    }

    public static AstronautDetail ForPerson(
        int personId,
        string currentRank,
        string currentDutyTitle,
        DateTime careerStartDate,
        DateTime? careerEndDate) =>
        new (
            id: default,
            personId: personId,
            currentRank: currentRank,
            currentDutyTitle: currentDutyTitle,
            careerStartDate: careerStartDate,
            careerEndDate: careerEndDate);

    private AstronautDetail(
        int id,
        int personId,
        string currentRank,
        string currentDutyTitle,
        DateTime careerStartDate,
        DateTime? careerEndDate)
    {
        Id = id;
        PersonId = personId;
        CurrentRank = currentRank;
        CurrentDutyTitle = currentDutyTitle;
        CareerStartDate = careerStartDate;
        CareerEndDate = careerEndDate;
    }

    [Obsolete("Provided exclusively for seeding development environment data.", error: false)]
    public static AstronautDetail Seed(
        int id,
        int personId,
        string currentRank,
        string currentDutyTitle,
        DateTime careerStartDate,
        DateTime? careerEndDate) =>
        new (
            id: id,
            personId: personId,
            currentRank: currentRank,
            currentDutyTitle: currentDutyTitle,
            careerStartDate: careerStartDate,
            careerEndDate: careerEndDate);
}

public class AstronautDetailConfiguration : IEntityTypeConfiguration<AstronautDetail>
{
    public void Configure(EntityTypeBuilder<AstronautDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}
