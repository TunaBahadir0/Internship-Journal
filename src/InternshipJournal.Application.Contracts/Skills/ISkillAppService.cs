using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace InternshipJournal.Skills;

public interface ISkillAppService : IApplicationService
{
    Task<List<SkillLookupDto>> GetListAsync();
}
