using System;
using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.DailyLogs;

public class CreateDailyLogDto
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime LogDate { get; set; }

    [StringLength(DailyLogConsts.MaxSummaryLength)]
    public string? Summary { get; set; }
}
