using System;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.DailyLogs;

public class ProblemSolvingEntryDto : EntityDto<Guid>
{
    public string Title { get; set; } = null!;

    public string ProblemDescription { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public string? AttemptedSolutions { get; set; }

    public string? RootCause { get; set; }

    public string? FinalSolution { get; set; }

    public bool UsedArtificialIntelligence { get; set; }

    public string? AiToolName { get; set; }

    public string? AiPromptSummary { get; set; }

    public string? AiSuggestion { get; set; }

    public bool? AiSuggestionAccepted { get; set; }

    public string? AiRejectionReason { get; set; }
}
