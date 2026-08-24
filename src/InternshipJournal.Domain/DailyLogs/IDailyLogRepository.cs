using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InternshipJournal.Enums;
using Volo.Abp.Domain.Repositories;

namespace InternshipJournal.DailyLogs;

public interface IDailyLogRepository : IRepository<DailyLog, Guid>
{
    Task<bool> ExistsForDateAsync(Guid internProfileId, DateTime logDate);

    Task<DailyLog?> GetByInternAndDateAsync(Guid internProfileId, DateTime logDate);

    Task<DailyLog?> GetWithDetailsAsync(Guid id);

    Task<List<DailyLog>> GetListWithDetailsAsync(
        Guid? internProfileId = null,
        DailyLogStatus? status = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue);
}
