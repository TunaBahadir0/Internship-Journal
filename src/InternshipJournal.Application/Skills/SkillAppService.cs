using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace InternshipJournal.Skills;

public class SkillAppService : InternshipJournalAppService, ISkillAppService
{
    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly InternshipJournalApplicationMappers _mapper;

    public SkillAppService(IRepository<Skill, Guid> skillRepository, InternshipJournalApplicationMappers mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    public async Task<List<SkillLookupDto>> GetListAsync()
    {
        var skills = await _skillRepository.GetListAsync(x => x.IsActive);

        return skills
            .OrderBy(x => x.Name)
            .Select(x => _mapper.Map(x))
            .ToList();
    }
}
