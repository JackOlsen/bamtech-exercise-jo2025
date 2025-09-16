namespace StargateAPI.Business.Dtos;

public class PersonAstronaut
{
    public int PersonId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CurrentRank { get; init; } = string.Empty;
    public string CurrentDutyTitle { get; init; } = string.Empty;
    public DateTime? CareerStartDate { get; init; }
    public DateTime? CareerEndDate { get; init; }
}
