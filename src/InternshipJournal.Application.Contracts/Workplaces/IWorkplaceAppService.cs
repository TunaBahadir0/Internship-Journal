using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InternshipJournal.Workplaces;

public interface IWorkplaceAppService : IApplicationService
{
    Task<WorkplaceDetailDto> GetAsync(Guid id);

    Task<PagedResultDto<WorkplaceDto>> GetListAsync(GetWorkplaceListInput input);

    Task<WorkplaceDetailDto> CreateAsync(CreateWorkplaceDto input);

    Task<WorkplaceDetailDto> UpdateAsync(Guid id, UpdateWorkplaceDto input);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);
}
