using System;
using InternshipJournal.Enums;

namespace InternshipJournal.InternProfiles;

public class InternProfileWithDetails
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public Guid MentorUserId { get; set; }

    public string MentorFullName { get; set; } = null!;

    public Guid WorkplaceId { get; set; }

    public string WorkplaceName { get; set; } = null!;

    public string DistrictName { get; set; } = null!;

    public string ProvinceName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public string AddressLine { get; set; } = null!;

    public string University { get; set; } = null!;

    public string SchoolDepartment { get; set; } = null!;

    public string StudentNumber { get; set; } = null!;

    public DateTime InternshipStartDate { get; set; }

    public DateTime InternshipEndDate { get; set; }

    public int RequiredWorkDays { get; set; }

    public InternshipStatus Status { get; set; }
}
