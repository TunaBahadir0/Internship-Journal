using System;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace InternshipJournal.Workplaces;

[Authorize(InternshipJournalPermissions.Workplaces.Default)]
public class WorkplaceAppService : InternshipJournalAppService, IWorkplaceAppService
{
    private readonly IWorkplaceRepository _workplaceRepository;
    private readonly WorkplaceManager _workplaceManager;
    private readonly InternshipJournalApplicationMappers _mapper;

    public WorkplaceAppService(
        IWorkplaceRepository workplaceRepository,
        WorkplaceManager workplaceManager,
        InternshipJournalApplicationMappers mapper)
    {
        _workplaceRepository = workplaceRepository;
        _workplaceManager = workplaceManager;
        _mapper = mapper;
    }

    public async Task<WorkplaceDetailDto> GetAsync(Guid id)
    {
        var workplace = await _workplaceRepository.GetWithLocationAsync(id)
            ?? throw new EntityNotFoundException(typeof(Workplace), id);

        return _mapper.MapToWorkplaceDetailDto(workplace);
    }

    public async Task<PagedResultDto<WorkplaceDto>> GetListAsync(GetWorkplaceListInput input)
    {
        var queryable = await _workplaceRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x => x.Name.Contains(input.Filter));
        }

        if (input.IsActive.HasValue)
        {
            queryable = queryable.Where(x => x.IsActive == input.IsActive.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var items = await _workplaceRepository.GetListWithLocationAsync(
            input.Filter,
            input.IsActive,
            input.SkipCount,
            input.MaxResultCount);

        return new PagedResultDto<WorkplaceDto>(
            totalCount,
            items.Select(_mapper.MapToWorkplaceDto).ToList());
    }

    [Authorize(InternshipJournalPermissions.Workplaces.Create)]
    public async Task<WorkplaceDetailDto> CreateAsync(CreateWorkplaceDto input)
    {
        var workplace = await _workplaceManager.CreateAsync(
            input.Name,
            input.DistrictId,
            input.AddressLine,
            input.PostalCode,
            input.TaxNumber,
            input.Phone,
            input.Email,
            input.Website,
            input.Latitude,
            input.Longitude);

        await _workplaceRepository.InsertAsync(workplace, autoSave: true);

        return await GetAsync(workplace.Id);
    }

    [Authorize(InternshipJournalPermissions.Workplaces.Edit)]
    public async Task<WorkplaceDetailDto> UpdateAsync(Guid id, UpdateWorkplaceDto input)
    {
        var workplace = await _workplaceRepository.GetAsync(id);

        await _workplaceManager.ChangeNameAsync(workplace, input.Name);
        await _workplaceManager.ChangeAddressAsync(workplace, input.DistrictId, input.AddressLine, input.PostalCode);

        workplace.ChangeContactInformation(input.TaxNumber, input.Phone, input.Email, input.Website);
        workplace.ChangeCoordinates(input.Latitude, input.Longitude);

        await _workplaceRepository.UpdateAsync(workplace, autoSave: true);

        return await GetAsync(id);
    }

    [Authorize(InternshipJournalPermissions.Workplaces.Edit)]
    public async Task ActivateAsync(Guid id)
    {
        var workplace = await _workplaceRepository.GetAsync(id);
        workplace.Activate();
        await _workplaceRepository.UpdateAsync(workplace);
    }

    [Authorize(InternshipJournalPermissions.Workplaces.Edit)]
    public async Task DeactivateAsync(Guid id)
    {
        var workplace = await _workplaceRepository.GetAsync(id);
        workplace.Deactivate();
        await _workplaceRepository.UpdateAsync(workplace);
    }
}
