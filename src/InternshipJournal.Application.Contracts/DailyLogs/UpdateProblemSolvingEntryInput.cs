using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.DailyLogs;

public class UpdateProblemSolvingEntryInput
{
    [Required]
    [StringLength(ProblemSolvingEntryConsts.MaxTitleLength)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(ProblemSolvingEntryConsts.MaxProblemDescriptionLength)]
    public string ProblemDescription { get; set; } = null!;

    [StringLength(ProblemSolvingEntryConsts.MaxErrorMessageLength)]
    public string? ErrorMessage { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxAttemptedSolutionsLength)]
    public string? AttemptedSolutions { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxRootCauseLength)]
    public string? RootCause { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxFinalSolutionLength)]
    public string? FinalSolution { get; set; }

    public bool UsedArtificialIntelligence { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxAiToolNameLength)]
    public string? AiToolName { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxAiPromptSummaryLength)]
    public string? AiPromptSummary { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxAiSuggestionLength)]
    public string? AiSuggestion { get; set; }

    public bool? AiSuggestionAccepted { get; set; }

    [StringLength(ProblemSolvingEntryConsts.MaxAiRejectionReasonLength)]
    public string? AiRejectionReason { get; set; }
}
