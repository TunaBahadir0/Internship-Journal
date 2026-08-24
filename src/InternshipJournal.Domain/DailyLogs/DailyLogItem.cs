using System;
using InternshipJournal.Consts;
using InternshipJournal.Enums;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace InternshipJournal.DailyLogs;

public class DailyLogItem : Entity<Guid>
{
    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public WorkType WorkType { get; private set; }

    public int DurationMinutes { get; private set; }

    public bool IsCompleted { get; private set; }

    protected DailyLogItem()
    {
        /* Required by EF Core. */
    }

    internal DailyLogItem(
        Guid id,
        [NotNull] string title,
        [CanBeNull] string? description,
        WorkType workType,
        int durationMinutes,
        bool isCompleted) : base(id)
    {
        SetTitle(title);
        SetDescription(description);
        WorkType = workType;
        SetDurationMinutes(durationMinutes);
        IsCompleted = isCompleted;
    }

    internal void Update(
        [NotNull] string title,
        [CanBeNull] string? description,
        WorkType workType,
        int durationMinutes,
        bool isCompleted)
    {
        SetTitle(title);
        SetDescription(description);
        WorkType = workType;
        SetDurationMinutes(durationMinutes);
        IsCompleted = isCompleted;
    }

    private void SetTitle([NotNull] string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), DailyLogItemConsts.MaxTitleLength);
    }

    private void SetDescription([CanBeNull] string? description)
    {
        Description = Check.Length(description, nameof(description), DailyLogItemConsts.MaxDescriptionLength);
    }

    private void SetDurationMinutes(int durationMinutes)
    {
        if (durationMinutes <= 0)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogItemDurationMustBePositive);
        }

        DurationMinutes = durationMinutes;
    }
}
