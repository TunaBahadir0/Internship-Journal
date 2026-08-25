using System;
using System.Threading.Tasks;
using InternshipJournal.Data;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace InternshipJournal.Skills;

public abstract class SkillAppServiceTests<TStartupModule> : InternshipJournalApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ISkillAppService _skillAppService;
    private readonly IRepository<Skill, Guid> _skillRepository;

    protected SkillAppServiceTests()
    {
        _skillAppService = GetRequiredService<ISkillAppService>();
        _skillRepository = GetRequiredService<IRepository<Skill, Guid>>();
    }

    [Fact]
    public async Task GetList_ShouldReturnOnlyActiveSkills()
    {
        var result = await _skillAppService.GetListAsync();

        result.ShouldNotBeEmpty();
        result.ShouldContain(x => x.Id == InternshipJournalSeedIds.Skills.CSharp);
    }

    [Fact]
    public async Task GetList_ShouldNotReturnInactiveSkills()
    {
        var inactiveSkillId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var skill = new Skill(inactiveSkillId, "Test Yetkinliği Pasif", isActive: true);
            skill.Deactivate();
            await _skillRepository.InsertAsync(skill);
        });

        var result = await _skillAppService.GetListAsync();

        result.ShouldNotContain(x => x.Id == inactiveSkillId);
    }
}
