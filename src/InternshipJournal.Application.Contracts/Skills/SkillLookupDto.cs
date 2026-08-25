using System;

namespace InternshipJournal.Skills;

public class SkillLookupDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }
}
