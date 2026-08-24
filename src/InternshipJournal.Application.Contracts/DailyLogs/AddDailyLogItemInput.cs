using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;
using InternshipJournal.Enums;

namespace InternshipJournal.DailyLogs;

public class AddDailyLogItemInput
{
    [Required]
    [StringLength(DailyLogItemConsts.MaxTitleLength)]
    public string Title { get; set; } = null!;

    [StringLength(DailyLogItemConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public WorkType WorkType { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    public bool IsCompleted { get; set; }
}
