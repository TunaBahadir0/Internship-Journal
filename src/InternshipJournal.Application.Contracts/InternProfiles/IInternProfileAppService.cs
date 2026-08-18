using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InternshipJournal.InternProfiles;

public interface IInternProfileAppService : IApplicationService
{
    Task<InternProfileDetailDto> GetMyProfileAsync();

    Task<InternProfileDetailDto> GetAsync(Guid id);

    Task<PagedResultDto<InternProfileDto>> GetListAsync(GetInternProfileListInput input);

    Task<InternProfileDetailDto> CreateAsync(CreateInternProfileDto input);

    Task<InternProfileDetailDto> UpdateAsync(Guid id, UpdateInternProfileDto input);

    Task StartAsync(Guid id);

    Task CompleteAsync(Guid id);

    Task CancelAsync(Guid id);
}
