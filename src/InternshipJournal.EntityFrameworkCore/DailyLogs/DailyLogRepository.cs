using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.EntityFrameworkCore;
using InternshipJournal.Enums;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace InternshipJournal.DailyLogs;

public class DailyLogRepository : EfCoreRepository<InternshipJournalDbContext, DailyLog, Guid>, IDailyLogRepository
{
    public DailyLogRepository(IDbContextProvider<InternshipJournalDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<bool> ExistsForDateAsync(Guid internProfileId, DateTime logDate)
    {
        var dbSet = await GetDbSetAsync();
        var date = logDate.Date;

        return await dbSet.AnyAsync(x => x.InternProfileId == internProfileId && x.LogDate == date);
    }

    public async Task<DailyLog?> GetByInternAndDateAsync(Guid internProfileId, DateTime logDate)
    {
        var dbSet = await GetDbSetAsync();
        var date = logDate.Date;

        return await dbSet.FirstOrDefaultAsync(x => x.InternProfileId == internProfileId && x.LogDate == date);
    }

    public async Task<DailyLog?> GetWithDetailsAsync(Guid id)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Include(x => x.Items)
            .Include(x => x.Skills)
            .Include(x => x.Problems)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // Not: İsme rağmen ("WithDetails") burada child collection'lar Include edilmiyor.
    // Bu metot liste ekranı içindir; 15. Gün'ün "Liste sorgusunda child collection'ların
    // tamamı yüklenmez" kuralı gereği yalnızca DailyLogDto'nun ihtiyaç duyduğu alanları
    // taşıyan DailyLog'ları döndürür. Tekil detay için GetWithDetailsAsync kullanılır.
    public async Task<List<DailyLog>> GetListWithDetailsAsync(
        Guid? internProfileId = null,
        DailyLogStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? keyword = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsQueryable();

        if (internProfileId.HasValue)
        {
            query = query.Where(x => x.InternProfileId == internProfileId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(x => x.LogDate >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.LogDate <= endDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Summary != null && x.Summary.Contains(keyword));
        }

        return await query
            .OrderByDescending(x => x.LogDate)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }
}
