using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class DailyLogDto : EntityDto<Guid>
{
    public DateTime LogDate { get; set; }

    public string? Summary { get; set; }

    public int TotalMinutes { get; set; }

    public DailyLogStatus Status { get; set; }

    public DateTime? SubmittedAt { get; set; }
}
