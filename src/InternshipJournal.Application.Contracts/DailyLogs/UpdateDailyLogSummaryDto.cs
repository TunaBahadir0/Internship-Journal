using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.DailyLogs;

public class UpdateDailyLogSummaryDto
{
    [StringLength(DailyLogConsts.MaxSummaryLength)]
    public string? Summary { get; set; }
}
