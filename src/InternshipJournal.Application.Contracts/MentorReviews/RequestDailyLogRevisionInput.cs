using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.MentorReviews;

public class RequestDailyLogRevisionInput
{
    [Required]
    [StringLength(MentorReviewConsts.MaxCommentLength)]
    public string Comment { get; set; } = null!;
}
