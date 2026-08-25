using System;
using InternshipJournal.Enums;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace InternshipJournal.MentorReviews;

public class MentorReviewTests
{
    [Fact]
    public void Approve_ShouldCreateWithApprovedDecision()
    {
        var review = MentorReview.Approve(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "İyi iş.", DateTime.Now);

        review.Decision.ShouldBe(MentorReviewDecision.Approved);
        review.Comment.ShouldBe("İyi iş.");
    }

    [Fact]
    public void Approve_WhenCommentNull_ShouldCreate()
    {
        var review = MentorReview.Approve(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, DateTime.Now);

        review.Decision.ShouldBe(MentorReviewDecision.Approved);
        review.Comment.ShouldBeNull();
    }

    [Fact]
    public void RequestRevision_ShouldCreateWithRevisionRequestedDecision()
    {
        var review = MentorReview.RequestRevision(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Eksik madde var.", DateTime.Now);

        review.Decision.ShouldBe(MentorReviewDecision.RevisionRequested);
        review.Comment.ShouldBe("Eksik madde var.");
    }

    [Fact]
    public void RequestRevision_WhenCommentEmpty_ShouldFail()
    {
        Assert.Throws<BusinessException>(() =>
            MentorReview.RequestRevision(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "  ", DateTime.Now));
    }
}
