using System;
using InternshipJournal.Enums;

namespace InternshipJournal.DailyLogs;

/// <summary>
/// Mentorun "bekleyen incelemeler" listesi için DailyLog + InternProfile + kullanıcı
/// join'inin salt okunur projeksiyonu. Aggregate değil, yalnızca liste ekranı içindir.
/// </summary>
public class DailyLogForReview
{
    public Guid Id { get; set; }

    public Guid InternProfileId { get; set; }

    public string InternUserName { get; set; } = null!;

    public string InternFullName { get; set; } = null!;

    public DateTime LogDate { get; set; }

    public int TotalMinutes { get; set; }

    public DailyLogStatus Status { get; set; }

    public DateTime? SubmittedAt { get; set; }
}
