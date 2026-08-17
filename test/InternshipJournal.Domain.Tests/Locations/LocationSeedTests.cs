using System.Threading.Tasks;
using InternshipJournal.Skills;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace InternshipJournal.Locations;

public abstract class LocationSeedTests<TStartupModule> : InternshipJournalDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDataSeeder _dataSeeder;
    private readonly IRepository<Country, System.Guid> _countryRepository;
    private readonly IRepository<Province, System.Guid> _provinceRepository;
    private readonly IRepository<District, System.Guid> _districtRepository;
    private readonly IRepository<Skill, System.Guid> _skillRepository;

    protected LocationSeedTests()
    {
        _dataSeeder = GetRequiredService<IDataSeeder>();
        _countryRepository = GetRequiredService<IRepository<Country, System.Guid>>();
        _provinceRepository = GetRequiredService<IRepository<Province, System.Guid>>();
        _districtRepository = GetRequiredService<IRepository<District, System.Guid>>();
        _skillRepository = GetRequiredService<IRepository<Skill, System.Guid>>();
    }

    [Fact]
    public async Task Seed_WhenExecutedTwice_ShouldNotCreateDuplicates()
    {
        long countryCountBefore = 0, provinceCountBefore = 0, districtCountBefore = 0, skillCountBefore = 0;

        await WithUnitOfWorkAsync(async () =>
        {
            countryCountBefore = await _countryRepository.GetCountAsync();
            provinceCountBefore = await _provinceRepository.GetCountAsync();
            districtCountBefore = await _districtRepository.GetCountAsync();
            skillCountBefore = await _skillRepository.GetCountAsync();
        });

        await _dataSeeder.SeedAsync();

        await WithUnitOfWorkAsync(async () =>
        {
            (await _countryRepository.GetCountAsync()).ShouldBe(countryCountBefore);
            (await _provinceRepository.GetCountAsync()).ShouldBe(provinceCountBefore);
            (await _districtRepository.GetCountAsync()).ShouldBe(districtCountBefore);
            (await _skillRepository.GetCountAsync()).ShouldBe(skillCountBefore);
        });
    }
}
