using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("Person")]
public class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public virtual AstronautDetail? AstronautDetail { get; set; }

    public virtual ICollection<AstronautDuty> AstronautDuties { get; set; } = new HashSet<AstronautDuty>();

    [Obsolete("For EF use only", error: true)]
    protected Person() { }

    public Person(string name)
    {
        Name = name;
    }

    [Obsolete("Provided exclusively for seeding development environment data.", error: false)]
    public Person(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasOne(z => z.AstronautDetail).WithOne(z => z.Person).HasForeignKey<AstronautDetail>(z => z.PersonId);
        builder.HasMany(z => z.AstronautDuties).WithOne(z => z.Person).HasForeignKey(z => z.PersonId);
    }
}
