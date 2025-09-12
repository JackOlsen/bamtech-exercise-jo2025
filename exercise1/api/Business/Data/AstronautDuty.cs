using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("AstronautDuty")]
public class AstronautDuty
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public string Rank { get; set; } = string.Empty;

    public string DutyTitle { get; set; } = string.Empty;

    public DateTime DutyStartDate { get; set; }

    public DateTime? DutyEndDate { get; set; }

    public virtual Person Person { get; set; } = null!;

    [Obsolete("For EF use only", error: true)]
    protected AstronautDuty() { }

    public AstronautDuty(
        int personId,
        string rank,
        string dutyTitle,
        DateTime dutyStartDate,
        DateTime? dutyEndDate)
    {
        PersonId = personId;
        Rank = rank;
        DutyTitle = dutyTitle;
        DutyStartDate = dutyStartDate;
        DutyEndDate = dutyEndDate;
    }

    [Obsolete("Provided exclusively for seeding development environment data.", error: false)]
    public AstronautDuty(
        int id,
        int personId,
        string rank,
        string dutyTitle,
        DateTime dutyStartDate,
        DateTime? dutyEndDate)
    {
        Id = id;
        PersonId = personId;
        Rank = rank;
        DutyTitle = dutyTitle;
        DutyStartDate = dutyStartDate;
        DutyEndDate = dutyEndDate;
    }
}

public class AstronautDutyConfiguration : IEntityTypeConfiguration<AstronautDuty>
{
    public void Configure(EntityTypeBuilder<AstronautDuty> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}
