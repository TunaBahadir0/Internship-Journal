using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InternshipJournal.DailyLogs;

public interface IDailyLogAppService : IApplicationService
{
    Task<DailyLogDetailDto> GetAsync(Guid id);

    Task<PagedResultDto<DailyLogDto>> GetListAsync(GetDailyLogListInput input);

    /// <summary>
    /// Giriş yapan mentora bağlı stajyerlerin günlüklerini döndürür (varsayılan: yalnızca
    /// Submitted durumundakiler — "bekleyen incelemeler" listesi).
    /// </summary>
    Task<List<DailyLogForReviewDto>> GetListForReviewAsync(DailyLogStatus? status = null);

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

    // Approve/RequestRevision buradan kaldırıldı — bunlar artık yalnızca
    // IMentorReviewAppService üzerinden (mentor yetki kontrolüyle ve MentorReview
    // kaydı oluşturularak) yapılabiliyor. Bkz. Gün 19 günlüğü.

    Task<DailyLogDetailDto> ReturnToDraftAsync(Guid id);
}
