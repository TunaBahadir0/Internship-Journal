using System;
using InternshipJournal.Consts;
using InternshipJournal.Enums;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace InternshipJournal.DailyLogs;

public class DailyLogSkill : Entity<Guid>
{
    public Guid SkillId { get; private set; }

    public LearningLevel LearningLevel { get; private set; }

    public string? Note { get; private set; }

    protected DailyLogSkill()
    {
        /* Required by EF Core. */
    }

    internal DailyLogSkill(
        Guid id,
        Guid skillId,
        LearningLevel learningLevel,
        [CanBeNull] string? note) : base(id)
    {
        SkillId = skillId;
        LearningLevel = learningLevel;
        SetNote(note);
    }

    internal void Update(LearningLevel learningLevel, [CanBeNull] string? note)
    {
        LearningLevel = learningLevel;
        SetNote(note);
    }

    private void SetNote([CanBeNull] string? note)
    {
        Note = Check.Length(note, nameof(note), DailyLogSkillConsts.MaxNoteLength);
    }
}
