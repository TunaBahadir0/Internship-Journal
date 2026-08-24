using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class DailyLogSkillDto : EntityDto<Guid>
{
    public Guid SkillId { get; set; }

    public LearningLevel LearningLevel { get; set; }

    public string? Note { get; set; }
}
