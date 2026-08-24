using System;
using System.Collections.Generic;
using System.Linq;
using InternshipJournal.Consts;
using InternshipJournal.Enums;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.DailyLogs;

public class DailyLog : FullAuditedAggregateRoot<Guid>
{
    public Guid InternProfileId { get; private set; }

    public DateTime LogDate { get; private set; }

    public string? Summary { get; private set; }

    public int TotalMinutes { get; private set; }

    public DailyLogStatus Status { get; private set; }

    public DateTime? SubmittedAt { get; private set; }

    public DateTime? ReviewedAt { get; private set; }

    public DateTime? ApprovedAt { get; private set; }

    private readonly List<DailyLogItem> _items = [];
    public IReadOnlyCollection<DailyLogItem> Items => _items;

    private readonly List<DailyLogSkill> _skills = [];
    public IReadOnlyCollection<DailyLogSkill> Skills => _skills;

    private readonly List<ProblemSolvingEntry> _problems = [];
    public IReadOnlyCollection<ProblemSolvingEntry> Problems => _problems;

    protected DailyLog()
    {
        /* Required by EF Core. */
    }

    internal DailyLog(
        Guid id,
        Guid internProfileId,
        DateTime logDate,
        [CanBeNull] string? summary) : base(id)
    {
        InternProfileId = internProfileId;
        LogDate = logDate.Date;
        Summary = Check.Length(summary, nameof(summary), DailyLogConsts.MaxSummaryLength);
        TotalMinutes = 0;
        Status = DailyLogStatus.Draft;
    }

    public void ChangeSummary([CanBeNull] string? summary)
    {
        EnsureEditable();

        Summary = Check.Length(summary, nameof(summary), DailyLogConsts.MaxSummaryLength);
    }

    public void AddItem(
        [NotNull] string title,
        [CanBeNull] string? description,
        WorkType workType,
        int durationMinutes,
        bool isCompleted)
    {
        EnsureEditable();

        _items.Add(new DailyLogItem(Guid.NewGuid(), title, description, workType, durationMinutes, isCompleted));
        RecalculateTotalMinutes();
    }

    public void UpdateItem(
        Guid itemId,
        [NotNull] string title,
        [CanBeNull] string? description,
        WorkType workType,
        int durationMinutes,
        bool isCompleted)
    {
        EnsureEditable();

        var item = GetItem(itemId);
        item.Update(title, description, workType, durationMinutes, isCompleted);
        RecalculateTotalMinutes();
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureEditable();

        var item = GetItem(itemId);
        _items.Remove(item);
        RecalculateTotalMinutes();
    }

    public void AddSkill(Guid skillId, LearningLevel learningLevel, [CanBeNull] string? note)
    {
        EnsureEditable();

        if (_skills.Any(x => x.SkillId == skillId))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DuplicateDailyLogSkill);
        }

        _skills.Add(new DailyLogSkill(Guid.NewGuid(), skillId, learningLevel, note));
    }

    public void UpdateSkill(Guid skillEntryId, LearningLevel learningLevel, [CanBeNull] string? note)
    {
        EnsureEditable();

        var skill = GetSkill(skillEntryId);
        skill.Update(learningLevel, note);
    }

    public void RemoveSkill(Guid skillEntryId)
    {
        EnsureEditable();

        var skill = GetSkill(skillEntryId);
        _skills.Remove(skill);
    }

    public void AddProblem(
        [NotNull] string title,
        [NotNull] string problemDescription,
        [CanBeNull] string? errorMessage,
        [CanBeNull] string? attemptedSolutions,
        [CanBeNull] string? rootCause,
        [CanBeNull] string? finalSolution,
        bool usedArtificialIntelligence,
        [CanBeNull] string? aiToolName,
        [CanBeNull] string? aiPromptSummary,
        [CanBeNull] string? aiSuggestion,
        bool? aiSuggestionAccepted,
        [CanBeNull] string? aiRejectionReason)
    {
        EnsureEditable();

        _problems.Add(new ProblemSolvingEntry(
            Guid.NewGuid(),
            title,
            problemDescription,
            errorMessage,
            attemptedSolutions,
            rootCause,
            finalSolution,
            usedArtificialIntelligence,
            aiToolName,
            aiPromptSummary,
            aiSuggestion,
            aiSuggestionAccepted,
            aiRejectionReason));
    }

    public void UpdateProblem(
        Guid problemId,
        [NotNull] string title,
        [NotNull] string problemDescription,
        [CanBeNull] string? errorMessage,
        [CanBeNull] string? attemptedSolutions,
        [CanBeNull] string? rootCause,
        [CanBeNull] string? finalSolution,
        bool usedArtificialIntelligence,
        [CanBeNull] string? aiToolName,
        [CanBeNull] string? aiPromptSummary,
        [CanBeNull] string? aiSuggestion,
        bool? aiSuggestionAccepted,
        [CanBeNull] string? aiRejectionReason)
    {
        EnsureEditable();

        var problem = GetProblem(problemId);
        problem.Update(
            title,
            problemDescription,
            errorMessage,
            attemptedSolutions,
            rootCause,
            finalSolution,
            usedArtificialIntelligence,
            aiToolName,
            aiPromptSummary,
            aiSuggestion,
            aiSuggestionAccepted,
            aiRejectionReason);
    }

    public void RemoveProblem(Guid problemId)
    {
        EnsureEditable();

        var problem = GetProblem(problemId);
        _problems.Remove(problem);
    }

    public void Submit()
    {
        if (Status != DailyLogStatus.Draft)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogCannotBeSubmitted);
        }

        if (_items.Count == 0)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogMustHaveAtLeastOneItem);
        }

        Status = DailyLogStatus.Submitted;
        SubmittedAt = DateTime.Now;
    }

    public void Approve()
    {
        if (Status != DailyLogStatus.Submitted)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogCannotBeApproved);
        }

        Status = DailyLogStatus.Approved;
        ReviewedAt = DateTime.Now;
        ApprovedAt = DateTime.Now;
    }

    public void RequestRevision()
    {
        if (Status != DailyLogStatus.Submitted)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogCannotRequestRevision);
        }

        Status = DailyLogStatus.RevisionRequested;
        ReviewedAt = DateTime.Now;
    }

    public void ReturnToDraft()
    {
        if (Status != DailyLogStatus.RevisionRequested)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogCannotReturnToDraft);
        }

        Status = DailyLogStatus.Draft;
    }

    private DailyLogItem GetItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogItemNotFound);
        }

        return item;
    }

    private DailyLogSkill GetSkill(Guid skillEntryId)
    {
        var skill = _skills.FirstOrDefault(x => x.Id == skillEntryId);
        if (skill == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogSkillEntryNotFound);
        }

        return skill;
    }

    private ProblemSolvingEntry GetProblem(Guid problemId)
    {
        var problem = _problems.FirstOrDefault(x => x.Id == problemId);
        if (problem == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogProblemEntryNotFound);
        }

        return problem;
    }

    private void RecalculateTotalMinutes()
    {
        TotalMinutes = _items.Sum(x => x.DurationMinutes);
    }

    private void EnsureEditable()
    {
        if (Status is DailyLogStatus.Submitted or DailyLogStatus.Approved)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogCannotBeEdited);
        }
    }
}
