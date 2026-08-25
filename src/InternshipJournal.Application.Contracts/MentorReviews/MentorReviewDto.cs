using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.MentorReviews;

public class MentorReviewDto : EntityDto<Guid>
{
    public Guid DailyLogId { get; set; }

    public Guid MentorUserId { get; set; }

    public MentorReviewDecision Decision { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewedAt { get; set; }
}
