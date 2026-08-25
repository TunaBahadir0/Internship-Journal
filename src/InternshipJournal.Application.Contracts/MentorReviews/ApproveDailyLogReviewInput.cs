using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.MentorReviews;

public class ApproveDailyLogReviewInput
{
    [StringLength(MentorReviewConsts.MaxCommentLength)]
    public string? Comment { get; set; }
}
