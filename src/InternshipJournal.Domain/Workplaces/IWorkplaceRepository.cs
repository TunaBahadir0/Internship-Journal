using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace InternshipJournal.Workplaces;

public interface IWorkplaceRepository : IRepository<Workplace, Guid>
{
    Task<Workplace?> FindByNameAsync(string name, Guid? excludedId = null);

    Task<bool> IsNameInUseAsync(string name, Guid? excludedId = null);

    Task<WorkplaceWithLocation?> GetWithLocationAsync(Guid id);

    Task<List<WorkplaceWithLocation>> GetListWithLocationAsync(
        string? filter = null,
        bool? isActive = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue);
}
