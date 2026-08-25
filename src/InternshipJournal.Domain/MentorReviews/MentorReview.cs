using System;
using InternshipJournal.Consts;
using InternshipJournal.Enums;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.MentorReviews;

public class MentorReview : FullAuditedAggregateRoot<Guid>
{
    public Guid DailyLogId { get; private set; }

    public Guid MentorUserId { get; private set; }

    public MentorReviewDecision Decision { get; private set; }

    public string? Comment { get; private set; }

    public DateTime ReviewedAt { get; private set; }

    protected MentorReview()
    {
        /* Required by EF Core. */
    }

    private MentorReview(
        Guid id,
        Guid dailyLogId,
        Guid mentorUserId,
        MentorReviewDecision decision,
        [CanBeNull] string? comment,
        DateTime reviewedAt) : base(id)
    {
        DailyLogId = dailyLogId;
        MentorUserId = mentorUserId;
        Decision = decision;
        Comment = Check.Length(comment, nameof(comment), MentorReviewConsts.MaxCommentLength);
        ReviewedAt = reviewedAt;
    }

    internal static MentorReview Approve(
        Guid id,
        Guid dailyLogId,
        Guid mentorUserId,
        [CanBeNull] string? comment,
        DateTime reviewedAt)
    {
        return new MentorReview(id, dailyLogId, mentorUserId, MentorReviewDecision.Approved, comment, reviewedAt);
    }

    internal static MentorReview RequestRevision(
        Guid id,
        Guid dailyLogId,
        Guid mentorUserId,
        [NotNull] string comment,
        DateTime reviewedAt)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.MentorReviewCommentRequiredForRevision);
        }

        return new MentorReview(id, dailyLogId, mentorUserId, MentorReviewDecision.RevisionRequested, comment, reviewedAt);
    }
}
