using System;
using System.Collections.Generic;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class DailyLogDetailDto : EntityDto<Guid>
{
    public Guid InternProfileId { get; set; }

    public DateTime LogDate { get; set; }

    public string? Summary { get; set; }

    public int TotalMinutes { get; set; }

    public DailyLogStatus Status { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public List<DailyLogItemDto> Items { get; set; } = [];

    public List<DailyLogSkillDto> Skills { get; set; } = [];

    public List<ProblemSolvingEntryDto> Problems { get; set; } = [];
}
