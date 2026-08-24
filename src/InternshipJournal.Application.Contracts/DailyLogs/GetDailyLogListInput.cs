using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class GetDailyLogListInput : PagedAndSortedResultRequestDto
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DailyLogStatus? Status { get; set; }

    public string? Keyword { get; set; }
}
