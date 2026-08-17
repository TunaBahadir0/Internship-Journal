using System;

namespace InternshipJournal.Locations;

public class CountryLookupDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}
