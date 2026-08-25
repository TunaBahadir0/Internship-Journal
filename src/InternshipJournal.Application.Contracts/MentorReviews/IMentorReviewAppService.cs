using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace InternshipJournal.MentorReviews;

public interface IMentorReviewAppService : IApplicationService
{
    Task<List<MentorReviewDto>> GetListByDailyLogAsync(Guid dailyLogId);

    Task<MentorReviewDto> ApproveAsync(Guid dailyLogId, ApproveDailyLogReviewInput input);

    Task<MentorReviewDto> RequestRevisionAsync(Guid dailyLogId, RequestDailyLogRevisionInput input);
}
