using System;
using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;
using InternshipJournal.Enums;

namespace InternshipJournal.DailyLogs;

public class AddDailyLogSkillInput
{
    [Required]
    public Guid SkillId { get; set; }

    [Required]
    public LearningLevel LearningLevel { get; set; }

    [StringLength(DailyLogSkillConsts.MaxNoteLength)]
    public string? Note { get; set; }
}
