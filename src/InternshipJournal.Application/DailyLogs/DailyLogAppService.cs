using System;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.InternProfiles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;

namespace InternshipJournal.DailyLogs;

public class DailyLogAppService : InternshipJournalAppService, IDailyLogAppService
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IInternProfileRepository _internProfileRepository;
    private readonly DailyLogManager _dailyLogManager;
    private readonly InternshipJournalApplicationMappers _mapper;

    public DailyLogAppService(
        IDailyLogRepository dailyLogRepository,
        IInternProfileRepository internProfileRepository,
        DailyLogManager dailyLogManager,
        InternshipJournalApplicationMappers mapper)
    {
        _dailyLogRepository = dailyLogRepository;
        _internProfileRepository = internProfileRepository;
        _dailyLogManager = dailyLogManager;
        _mapper = mapper;
    }

    public async Task<DailyLogDetailDto> GetAsync(Guid id)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<PagedResultDto<DailyLogDto>> GetListAsync(GetDailyLogListInput input)
    {
        var internProfile = await GetCurrentActiveInternProfileAsync();

        var queryable = await _dailyLogRepository.GetQueryableAsync();
        queryable = ApplyFilter(queryable, internProfile.Id, input);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        // Liste sorgusu child collection'ları (Items/Skills/Problems) yüklemez;
        // DailyLogDto zaten yalnızca özet alanları taşıyor.
        var items = await _dailyLogRepository.GetListWithDetailsAsync(
            internProfile.Id,
            input.Status,
            input.StartDate,
            input.EndDate,
            input.Keyword,
            input.SkipCount,
            input.MaxResultCount);

        return new PagedResultDto<DailyLogDto>(
            totalCount,
            items.Select(_mapper.MapToDailyLogDto).ToList());
    }

    public async Task<DailyLogDetailDto> CreateAsync(CreateDailyLogDto input)
    {
        var internProfile = await GetCurrentActiveInternProfileAsync();

        var dailyLog = await _dailyLogManager.CreateAsync(internProfile.Id, input.LogDate, input.Summary);

        await _dailyLogRepository.InsertAsync(dailyLog, autoSave: true);

        return await GetAsync(dailyLog.Id);
    }

    public async Task<DailyLogDetailDto> UpdateSummaryAsync(Guid id, UpdateDailyLogSummaryDto input)
    {
        var dailyLog = await _dailyLogRepository.GetAsync(id);

        dailyLog.ChangeSummary(input.Summary);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return await GetAsync(id);
    }

    public async Task<DailyLogDetailDto> AddItemAsync(Guid id, AddDailyLogItemInput input)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.AddItem(input.Title, input.Description, input.WorkType, input.DurationMinutes, input.IsCompleted);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> UpdateItemAsync(Guid id, Guid itemId, UpdateDailyLogItemInput input)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.UpdateItem(itemId, input.Title, input.Description, input.WorkType, input.DurationMinutes, input.IsCompleted);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> RemoveItemAsync(Guid id, Guid itemId)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.RemoveItem(itemId);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> AddSkillAsync(Guid id, AddDailyLogSkillInput input)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        await _dailyLogManager.AddSkillAsync(dailyLog, input.SkillId, input.LearningLevel, input.Note);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> UpdateSkillAsync(Guid id, Guid skillEntryId, UpdateDailyLogSkillInput input)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.UpdateSkill(skillEntryId, input.LearningLevel, input.Note);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> RemoveSkillAsync(Guid id, Guid skillEntryId)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.RemoveSkill(skillEntryId);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> AddProblemAsync(Guid id, AddProblemSolvingEntryInput input)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.AddProblem(
            input.Title,
            input.ProblemDescription,
            input.ErrorMessage,
            input.AttemptedSolutions,
            input.RootCause,
            input.FinalSolution,
            input.UsedArtificialIntelligence,
            input.AiToolName,
            input.AiPromptSummary,
            input.AiSuggestion,
            input.AiSuggestionAccepted,
            input.AiRejectionReason);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> UpdateProblemAsync(Guid id, Guid problemId, UpdateProblemSolvingEntryInput input)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.UpdateProblem(
            problemId,
            input.Title,
            input.ProblemDescription,
            input.ErrorMessage,
            input.AttemptedSolutions,
            input.RootCause,
            input.FinalSolution,
            input.UsedArtificialIntelligence,
            input.AiToolName,
            input.AiPromptSummary,
            input.AiSuggestion,
            input.AiSuggestionAccepted,
            input.AiRejectionReason);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> RemoveProblemAsync(Guid id, Guid problemId)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.RemoveProblem(problemId);

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> SubmitAsync(Guid id)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.Submit();

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> RequestRevisionAsync(Guid id)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.RequestRevision();

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> ApproveAsync(Guid id)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.Approve();

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    public async Task<DailyLogDetailDto> ReturnToDraftAsync(Guid id)
    {
        var dailyLog = await GetWithDetailsOrThrowAsync(id);

        dailyLog.ReturnToDraft();

        await _dailyLogRepository.UpdateAsync(dailyLog, autoSave: true);

        return _mapper.MapToDailyLogDetailDto(dailyLog);
    }

    private async Task<DailyLog> GetWithDetailsOrThrowAsync(Guid id)
    {
        return await _dailyLogRepository.GetWithDetailsAsync(id)
            ?? throw new EntityNotFoundException(typeof(DailyLog), id);
    }

    private async Task<InternProfile> GetCurrentActiveInternProfileAsync()
    {
        return await _internProfileRepository.GetActiveByUserIdAsync(CurrentUser.GetId())
            ?? throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogInternProfileNotActive);
    }

    private static IQueryable<DailyLog> ApplyFilter(IQueryable<DailyLog> queryable, Guid internProfileId, GetDailyLogListInput input)
    {
        queryable = queryable.Where(x => x.InternProfileId == internProfileId);

        if (input.StartDate.HasValue)
        {
            queryable = queryable.Where(x => x.LogDate >= input.StartDate.Value.Date);
        }

        if (input.EndDate.HasValue)
        {
            queryable = queryable.Where(x => x.LogDate <= input.EndDate.Value.Date);
        }

        if (input.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == input.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            queryable = queryable.Where(x => x.Summary != null && x.Summary.Contains(input.Keyword));
        }

        return queryable;
    }
}
