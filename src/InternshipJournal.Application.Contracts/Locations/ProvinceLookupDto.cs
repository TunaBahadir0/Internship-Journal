using System;

namespace InternshipJournal.Locations;

public class ProvinceLookupDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }
}
