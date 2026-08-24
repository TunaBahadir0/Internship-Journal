using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class DailyLogItemDto : EntityDto<Guid>
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public WorkType WorkType { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsCompleted { get; set; }
}
