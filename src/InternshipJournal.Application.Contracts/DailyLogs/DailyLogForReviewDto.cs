using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class DailyLogForReviewDto : EntityDto<Guid>
{
    public Guid InternProfileId { get; set; }

    public string InternUserName { get; set; } = null!;

    public string InternFullName { get; set; } = null!;

    public DateTime LogDate { get; set; }

    public int TotalMinutes { get; set; }

    public DailyLogStatus Status { get; set; }

    public DateTime? SubmittedAt { get; set; }
}
