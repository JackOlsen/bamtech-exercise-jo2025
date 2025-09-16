using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("LogEntries")]
public class LogEntry
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Description { get; set; }
    public string Detail{ get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public long Elapsed { get; set; }

    [Obsolete("For serialization only", true)]
    protected LogEntry() { }

    public LogEntry(
        DateTimeOffset timestamp,
        string description,
        string detail,
        bool success,
        string? error,
        long elapsed)
    {
        Id = new Guid();
        Timestamp = timestamp;
        Description = description;
        Detail = detail;
        Success = success;
        Error = error;
        Elapsed = elapsed;
    }
}

public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
{
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }
}