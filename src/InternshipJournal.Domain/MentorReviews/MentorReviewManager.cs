using System;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using InternshipJournal.InternProfiles;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;

namespace InternshipJournal.MentorReviews;

public class MentorReviewManager : DomainService
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IInternProfileRepository _internProfileRepository;
    private readonly IClock _clock;

    public MentorReviewManager(
        IDailyLogRepository dailyLogRepository,
        IInternProfileRepository internProfileRepository,
        IClock clock)
    {
        _dailyLogRepository = dailyLogRepository;
        _internProfileRepository = internProfileRepository;
        _clock = clock;
    }

    public async Task<(MentorReview Review, DailyLog DailyLog)> ApproveAsync(Guid dailyLogId, Guid mentorUserId, string? comment)
    {
        var dailyLog = await GetAuthorizedDailyLogAsync(dailyLogId, mentorUserId);

        // Aggregate'i mutasyona uğratmadan önce MentorReview'i oluştur: RequestRevisionAsync'teki
        // yorum zorunluluğu gibi bir kural burada da geçerli olsaydı, günlüğün durumu hatalı bir
        // istekte bile değişmiş olmazdı (fail-fast, yan etkisiz doğrulama).
        var review = MentorReview.Approve(Guid.NewGuid(), dailyLogId, mentorUserId, comment, _clock.Now);

        dailyLog.Approve();

        return (review, dailyLog);
    }

    public async Task<(MentorReview Review, DailyLog DailyLog)> RequestRevisionAsync(Guid dailyLogId, Guid mentorUserId, string comment)
    {
        var dailyLog = await GetAuthorizedDailyLogAsync(dailyLogId, mentorUserId);

        var review = MentorReview.RequestRevision(Guid.NewGuid(), dailyLogId, mentorUserId, comment, _clock.Now);

        dailyLog.RequestRevision();

        return (review, dailyLog);
    }

    private async Task<DailyLog> GetAuthorizedDailyLogAsync(Guid dailyLogId, Guid mentorUserId)
    {
        var dailyLog = await _dailyLogRepository.GetWithDetailsAsync(dailyLogId)
            ?? throw new BusinessException(InternshipJournalDomainErrorCodes.MentorReviewDailyLogNotFound);

        var internProfile = await _internProfileRepository.GetAsync(dailyLog.InternProfileId);

        if (internProfile.MentorUserId != mentorUserId)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.MentorReviewNotAuthorized);
        }

        return dailyLog;
    }
}
