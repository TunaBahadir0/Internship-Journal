using System;
using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.InternProfiles;

public class CreateInternProfileDto
{
    [Required]
    public Guid UserId { get; set; }

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
    [DataType(DataType.Date)]
    public DateTime InternshipStartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime InternshipEndDate { get; set; }

    [Range(1, int.MaxValue)]
    public int RequiredWorkDays { get; set; }
}
