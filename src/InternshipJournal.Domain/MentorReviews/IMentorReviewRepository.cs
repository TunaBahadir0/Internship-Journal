using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace InternshipJournal.MentorReviews;

public interface IMentorReviewRepository : IRepository<MentorReview, Guid>
{
    Task<List<MentorReview>> GetListByDailyLogIdAsync(Guid dailyLogId);
}
