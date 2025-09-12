namespace StargateAPI.Business.Dtos;

// TODO: This is being used both as a data model and an API DTO.
// Split into separate classes.
public class PersonAstronaut
{
    public int PersonId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string CurrentRank { get; set; } = string.Empty;

    public string CurrentDutyTitle { get; set; } = string.Empty;

    public DateTime? CareerStartDate { get; set; }

    public DateTime? CareerEndDate { get; set; }
}
