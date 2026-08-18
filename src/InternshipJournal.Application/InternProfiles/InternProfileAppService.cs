using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;

namespace InternshipJournal.InternProfiles;

public class InternProfileAppService : InternshipJournalAppService, IInternProfileAppService
{
    private readonly IInternProfileRepository _internProfileRepository;
    private readonly InternProfileManager _internProfileManager;
    private readonly InternshipJournalApplicationMappers _mapper;

    public InternProfileAppService(
        IInternProfileRepository internProfileRepository,
        InternProfileManager internProfileManager,
        InternshipJournalApplicationMappers mapper)
    {
        _internProfileRepository = internProfileRepository;
        _internProfileManager = internProfileManager;
        _mapper = mapper;
    }

    public async Task<InternProfileDetailDto> GetMyProfileAsync()
    {
        var profile = await _internProfileRepository.FindByUserIdAsync(CurrentUser.GetId())
            ?? throw new EntityNotFoundException(typeof(InternProfile));

        return await GetAsync(profile.Id);
    }

    public async Task<InternProfileDetailDto> GetAsync(Guid id)
    {
        var profile = await _internProfileRepository.GetWithMentorAndWorkplaceAsync(id)
            ?? throw new EntityNotFoundException(typeof(InternProfile), id);

        return _mapper.MapToInternProfileDetailDto(profile);
    }

    public async Task<PagedResultDto<InternProfileDto>> GetListAsync(GetInternProfileListInput input)
    {
        var queryable = await _internProfileRepository.GetQueryableAsync();

        if (input.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await _internProfileRepository.GetListWithDetailsAsync(
            input.Filter,
            input.Status,
            input.SkipCount,
            input.MaxResultCount);

        return new PagedResultDto<InternProfileDto>(
            totalCount,
            items.Select(_mapper.MapToInternProfileDto).ToList());
    }

    public async Task<InternProfileDetailDto> CreateAsync(CreateInternProfileDto input)
    {
        var profile = await _internProfileManager.CreateAsync(
            input.UserId,
            input.MentorUserId,
            input.WorkplaceId,
            input.University,
            input.SchoolDepartment,
            input.StudentNumber,
            new DateRange(input.InternshipStartDate, input.InternshipEndDate),
            input.RequiredWorkDays);

        await _internProfileRepository.InsertAsync(profile, autoSave: true);

        return await GetAsync(profile.Id);
    }

    public async Task<InternProfileDetailDto> UpdateAsync(Guid id, UpdateInternProfileDto input)
    {
        var profile = await _internProfileRepository.GetAsync(id);

        await _internProfileManager.ChangeMentorAsync(profile, input.MentorUserId);
        await _internProfileManager.ChangeWorkplaceAsync(profile, input.WorkplaceId);

        profile.ChangeEducationInformation(input.University, input.SchoolDepartment, input.StudentNumber);
        profile.ChangeInternshipPeriod(new DateRange(input.InternshipStartDate, input.InternshipEndDate));
        profile.ChangeRequiredWorkDays(input.RequiredWorkDays);

        await _internProfileRepository.UpdateAsync(profile, autoSave: true);

        return await GetAsync(id);
    }

    public async Task StartAsync(Guid id)
    {
        var profile = await _internProfileRepository.GetAsync(id);
        profile.Start();
        await _internProfileRepository.UpdateAsync(profile);
    }

    public async Task CompleteAsync(Guid id)
    {
        var profile = await _internProfileRepository.GetAsync(id);
        profile.Complete();
        await _internProfileRepository.UpdateAsync(profile);
    }

    public async Task CancelAsync(Guid id)
    {
        var profile = await _internProfileRepository.GetAsync(id);
        profile.Cancel();
        await _internProfileRepository.UpdateAsync(profile);
    }
}
