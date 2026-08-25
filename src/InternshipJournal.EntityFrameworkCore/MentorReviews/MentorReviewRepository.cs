using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace InternshipJournal.MentorReviews;

public class MentorReviewRepository : EfCoreRepository<InternshipJournalDbContext, MentorReview, Guid>, IMentorReviewRepository
{
    public MentorReviewRepository(IDbContextProvider<InternshipJournalDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MentorReview>> GetListByDailyLogIdAsync(Guid dailyLogId)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.DailyLogId == dailyLogId)
            .OrderByDescending(x => x.ReviewedAt)
            .ToListAsync();
    }
}
