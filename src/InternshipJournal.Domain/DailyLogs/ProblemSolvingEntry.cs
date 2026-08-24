using System;
using InternshipJournal.Consts;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace InternshipJournal.DailyLogs;

public class ProblemSolvingEntry : Entity<Guid>
{
    public string Title { get; private set; } = null!;

    public string ProblemDescription { get; private set; } = null!;

    public string? ErrorMessage { get; private set; }

    public string? AttemptedSolutions { get; private set; }

    public string? RootCause { get; private set; }

    public string? FinalSolution { get; private set; }

    public bool UsedArtificialIntelligence { get; private set; }

    public string? AiToolName { get; private set; }

    public string? AiPromptSummary { get; private set; }

    public string? AiSuggestion { get; private set; }

    public bool? AiSuggestionAccepted { get; private set; }

    public string? AiRejectionReason { get; private set; }

    protected ProblemSolvingEntry()
    {
        /* Required by EF Core. */
    }

    internal ProblemSolvingEntry(
        Guid id,
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
        [CanBeNull] string? aiRejectionReason) : base(id)
    {
        SetTitle(title);
        SetProblemDescription(problemDescription);
        ErrorMessage = Check.Length(errorMessage, nameof(errorMessage), ProblemSolvingEntryConsts.MaxErrorMessageLength);
        AttemptedSolutions = Check.Length(attemptedSolutions, nameof(attemptedSolutions), ProblemSolvingEntryConsts.MaxAttemptedSolutionsLength);
        RootCause = Check.Length(rootCause, nameof(rootCause), ProblemSolvingEntryConsts.MaxRootCauseLength);
        FinalSolution = Check.Length(finalSolution, nameof(finalSolution), ProblemSolvingEntryConsts.MaxFinalSolutionLength);
        SetAiInformation(usedArtificialIntelligence, aiToolName, aiPromptSummary, aiSuggestion, aiSuggestionAccepted, aiRejectionReason);
    }

    internal void Update(
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
        SetTitle(title);
        SetProblemDescription(problemDescription);
        ErrorMessage = Check.Length(errorMessage, nameof(errorMessage), ProblemSolvingEntryConsts.MaxErrorMessageLength);
        AttemptedSolutions = Check.Length(attemptedSolutions, nameof(attemptedSolutions), ProblemSolvingEntryConsts.MaxAttemptedSolutionsLength);
        RootCause = Check.Length(rootCause, nameof(rootCause), ProblemSolvingEntryConsts.MaxRootCauseLength);
        FinalSolution = Check.Length(finalSolution, nameof(finalSolution), ProblemSolvingEntryConsts.MaxFinalSolutionLength);
        SetAiInformation(usedArtificialIntelligence, aiToolName, aiPromptSummary, aiSuggestion, aiSuggestionAccepted, aiRejectionReason);
    }

    private void SetTitle([NotNull] string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), ProblemSolvingEntryConsts.MaxTitleLength);
    }

    private void SetProblemDescription([NotNull] string problemDescription)
    {
        ProblemDescription = Check.NotNullOrWhiteSpace(problemDescription, nameof(problemDescription), ProblemSolvingEntryConsts.MaxProblemDescriptionLength);
    }

    private void SetAiInformation(
        bool usedArtificialIntelligence,
        [CanBeNull] string? aiToolName,
        [CanBeNull] string? aiPromptSummary,
        [CanBeNull] string? aiSuggestion,
        bool? aiSuggestionAccepted,
        [CanBeNull] string? aiRejectionReason)
    {
        if (usedArtificialIntelligence &&
            (string.IsNullOrWhiteSpace(aiToolName) || string.IsNullOrWhiteSpace(aiPromptSummary)))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.ProblemSolvingAiToolAndSummaryRequired);
        }

        if (aiSuggestionAccepted == false && string.IsNullOrWhiteSpace(aiRejectionReason))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.ProblemSolvingAiRejectionReasonRequired);
        }

        UsedArtificialIntelligence = usedArtificialIntelligence;
        AiToolName = Check.Length(aiToolName, nameof(aiToolName), ProblemSolvingEntryConsts.MaxAiToolNameLength);
        AiPromptSummary = Check.Length(aiPromptSummary, nameof(aiPromptSummary), ProblemSolvingEntryConsts.MaxAiPromptSummaryLength);
        AiSuggestion = Check.Length(aiSuggestion, nameof(aiSuggestion), ProblemSolvingEntryConsts.MaxAiSuggestionLength);
        AiSuggestionAccepted = aiSuggestionAccepted;
        AiRejectionReason = Check.Length(aiRejectionReason, nameof(aiRejectionReason), ProblemSolvingEntryConsts.MaxAiRejectionReasonLength);
    }
}
