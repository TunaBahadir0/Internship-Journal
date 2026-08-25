using System;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Timing;
using Xunit;

namespace InternshipJournal.MentorReviews;

public class MentorReviewManagerTests
{
    private static readonly DateTime Today = new(2026, 8, 26);

    private static InternProfile CreateInternProfile(Guid mentorUserId, Guid? internUserId = null)
    {
        var profile = new InternProfile(
            Guid.NewGuid(),
            internUserId ?? Guid.NewGuid(),
            mentorUserId,
            Guid.NewGuid(),
            "Test Üniversitesi",
            "Yazılım Mühendisliği",
            "12345678",
            new DateRange(new DateTime(2026, 8, 1), new DateTime(2026, 9, 30)),
            60);
        profile.Start();
        return profile;
    }

    private static DailyLog CreateSubmittedDailyLog(Guid internProfileId)
    {
        var log = new DailyLog(Guid.NewGuid(), internProfileId, Today, null);
        log.AddItem("Bir madde", null, WorkType.Development, 30, true);
        log.Submit();
        return log;
    }

    private static MentorReviewManager CreateManager(
        out IDailyLogRepository dailyLogRepository,
        out IInternProfileRepository internProfileRepository)
    {
        dailyLogRepository = Substitute.For<IDailyLogRepository>();
        internProfileRepository = Substitute.For<IInternProfileRepository>();

        var clock = Substitute.For<IClock>();
        clock.Now.Returns(Today);

        return new MentorReviewManager(dailyLogRepository, internProfileRepository, clock);
    }

    [Fact]
    public async Task ApproveAsync_WhenMentorMatches_ShouldApproveDailyLogAndCreateReview()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository);
        var mentorId = Guid.NewGuid();
        var profile = CreateInternProfile(mentorId);
        var dailyLog = CreateSubmittedDailyLog(profile.Id);

        dailyLogRepository.GetWithDetailsAsync(dailyLog.Id).Returns(dailyLog);
        internProfileRepository.GetAsync(profile.Id).ReturnsForAnyArgs(profile);

        var (review, updatedLog) = await manager.ApproveAsync(dailyLog.Id, mentorId, "Tebrikler.");

        review.Decision.ShouldBe(MentorReviewDecision.Approved);
        updatedLog.Status.ShouldBe(DailyLogStatus.Approved);
    }

    [Fact]
    public async Task RequestRevisionAsync_WhenMentorMatches_ShouldChangeDailyLogStatusAndCreateReview()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository);
        var mentorId = Guid.NewGuid();
        var profile = CreateInternProfile(mentorId);
        var dailyLog = CreateSubmittedDailyLog(profile.Id);

        dailyLogRepository.GetWithDetailsAsync(dailyLog.Id).Returns(dailyLog);
        internProfileRepository.GetAsync(profile.Id).ReturnsForAnyArgs(profile);

        var (review, updatedLog) = await manager.RequestRevisionAsync(dailyLog.Id, mentorId, "Eksik madde var.");

        review.Decision.ShouldBe(MentorReviewDecision.RevisionRequested);
        updatedLog.Status.ShouldBe(DailyLogStatus.RevisionRequested);
    }

    [Fact]
    public async Task ApproveAsync_WhenMentorDoesNotMatch_ShouldFail()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository);
        var profile = CreateInternProfile(Guid.NewGuid());
        var dailyLog = CreateSubmittedDailyLog(profile.Id);

        dailyLogRepository.GetWithDetailsAsync(dailyLog.Id).Returns(dailyLog);
        internProfileRepository.GetAsync(profile.Id).ReturnsForAnyArgs(profile);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.ApproveAsync(dailyLog.Id, Guid.NewGuid(), null));
    }

    [Fact]
    public async Task ApproveAsync_WhenDailyLogNotFound_ShouldFail()
    {
        var manager = CreateManager(out var dailyLogRepository, out _);
        dailyLogRepository.GetWithDetailsAsync(Arg.Any<Guid>()).ReturnsForAnyArgs((DailyLog?)null);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.ApproveAsync(Guid.NewGuid(), Guid.NewGuid(), null));
    }

    [Fact]
    public async Task ApproveAsync_WhenDailyLogNotSubmitted_ShouldFail()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository);
        var mentorId = Guid.NewGuid();
        var profile = CreateInternProfile(mentorId);
        var draftLog = new DailyLog(Guid.NewGuid(), profile.Id, Today, null);

        dailyLogRepository.GetWithDetailsAsync(draftLog.Id).Returns(draftLog);
        internProfileRepository.GetAsync(profile.Id).ReturnsForAnyArgs(profile);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.ApproveAsync(draftLog.Id, mentorId, null));
    }

    [Fact]
    public async Task RequestRevisionAsync_WhenCommentEmpty_ShouldFail()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository);
        var mentorId = Guid.NewGuid();
        var profile = CreateInternProfile(mentorId);
        var dailyLog = CreateSubmittedDailyLog(profile.Id);

        dailyLogRepository.GetWithDetailsAsync(dailyLog.Id).Returns(dailyLog);
        internProfileRepository.GetAsync(profile.Id).ReturnsForAnyArgs(profile);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.RequestRevisionAsync(dailyLog.Id, mentorId, " "));
    }
}
