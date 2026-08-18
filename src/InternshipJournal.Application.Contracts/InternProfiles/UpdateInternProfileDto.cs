using System;
using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.InternProfiles;

public class UpdateInternProfileDto
{
    [Required]
    public Guid MentorUserId { get; set; }

    [Required]
    public Guid WorkplaceId { get; set; }

    [Required]
    [StringLength(InternProfileConsts.MaxUniversityLength)]
    public string University { get; set; } = null!;

    [Required]
    [StringLength(InternProfileConsts.MaxSchoolDepartmentLength)]
    public string SchoolDepartment { get; set; } = null!;

    [Required]
    [StringLength(InternProfileConsts.MaxStudentNumberLength)]
    public string StudentNumber { get; set; } = null!;

    [Required]
    public DateTime InternshipStartDate { get; set; }

    [Required]
    public DateTime InternshipEndDate { get; set; }

    [Range(1, int.MaxValue)]
    public int RequiredWorkDays { get; set; }
}
