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

    Task<DailyLogDetailDto> AddItemAsync(Guid id, AddDailyLogItemInput input);

    Task<DailyLogDetailDto> UpdateItemAsync(Guid id, Guid itemId, UpdateDailyLogItemInput input);

    Task<DailyLogDetailDto> RemoveItemAsync(Guid id, Guid itemId);

    Task<DailyLogDetailDto> AddSkillAsync(Guid id, AddDailyLogSkillInput input);

    Task<DailyLogDetailDto> UpdateSkillAsync(Guid id, Guid skillEntryId, UpdateDailyLogSkillInput input);

    Task<DailyLogDetailDto> RemoveSkillAsync(Guid id, Guid skillEntryId);

    Task<DailyLogDetailDto> AddProblemAsync(Guid id, AddProblemSolvingEntryInput input);

    Task<DailyLogDetailDto> UpdateProblemAsync(Guid id, Guid problemId, UpdateProblemSolvingEntryInput input);

    Task<DailyLogDetailDto> RemoveProblemAsync(Guid id, Guid problemId);

    Task<DailyLogDetailDto> SubmitAsync(Guid id);

    Task<DailyLogDetailDto> RequestRevisionAsync(Guid id);

    Task<DailyLogDetailDto> ApproveAsync(Guid id);

    Task<DailyLogDetailDto> ReturnToDraftAsync(Guid id);
}
