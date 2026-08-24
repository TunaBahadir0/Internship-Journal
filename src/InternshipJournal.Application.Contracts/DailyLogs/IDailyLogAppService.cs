using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InternshipJournal.DailyLogs;

public interface IDailyLogAppService : IApplicationService
{
    Task<DailyLogDetailDto> GetAsync(Guid id);

    Task<PagedResultDto<DailyLogDto>> GetListAsync(GetDailyLogListInput input);

    Task<DailyLogDetailDto> CreateAsync(CreateDailyLogDto input);

    Task<DailyLogDetailDto> UpdateSummaryAsync(Guid id, UpdateDailyLogSummaryDto input);
}
