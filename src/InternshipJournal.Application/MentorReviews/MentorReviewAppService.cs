using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using Volo.Abp.Users;

namespace InternshipJournal.MentorReviews;

public class MentorReviewAppService : InternshipJournalAppService, IMentorReviewAppService
{
    private readonly MentorReviewManager _mentorReviewManager;
    private readonly IMentorReviewRepository _mentorReviewRepository;
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly InternshipJournalApplicationMappers _mapper;

    public MentorReviewAppService(
        MentorReviewManager mentorReviewManager,
        IMentorReviewRepository mentorReviewRepository,
        IDailyLogRepository dailyLogRepository,
        InternshipJournalApplicationMappers mapper)
    {
        _mentorReviewManager = mentorReviewManager;
        _mentorReviewRepository = mentorReviewRepository;
        _dailyLogRepository = dailyLogRepository;
        _mapper = mapper;
    }

    public async Task<List<MentorReviewDto>> GetListByDailyLogAsync(Guid dailyLogId)
    {
        var reviews = await _mentorReviewRepository.GetListByDailyLogIdAsync(dailyLogId);

        return reviews.Select(_mapper.Map).ToList();
    }

    public async Task<MentorReviewDto> ApproveAsync(Guid dailyLogId, ApproveDailyLogReviewInput input)
    {
        var (review, dailyLog) = await _mentorReviewManager.ApproveAsync(dailyLogId, CurrentUser.GetId(), input.Comment);

        await _mentorReviewRepository.InsertAsync(review, autoSave: true);
        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.Map(review);
    }

    public async Task<MentorReviewDto> RequestRevisionAsync(Guid dailyLogId, RequestDailyLogRevisionInput input)
    {
        var (review, dailyLog) = await _mentorReviewManager.RequestRevisionAsync(dailyLogId, CurrentUser.GetId(), input.Comment);

        await _mentorReviewRepository.InsertAsync(review, autoSave: true);
        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.Map(review);
    }
}
