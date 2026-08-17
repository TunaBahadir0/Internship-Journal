using System;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.Data;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace InternshipJournal.Locations;

public abstract class LocationAppServiceTests<TStartupModule> : InternshipJournalApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly ILocationAppService _locationAppService;
    private readonly IRepository<Country, Guid> _countryRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IRepository<District, Guid> _districtRepository;

    protected LocationAppServiceTests()
    {
        _locationAppService = GetRequiredService<ILocationAppService>();
        _countryRepository = GetRequiredService<IRepository<Country, Guid>>();
        _provinceRepository = GetRequiredService<IRepository<Province, Guid>>();
        _districtRepository = GetRequiredService<IRepository<District, Guid>>();
    }

    [Fact]
    public async Task GetCountries_ShouldReturnOnlyActiveCountries()
    {
        var result = await _locationAppService.GetCountriesAsync();

        result.ShouldNotBeEmpty();
        result.ShouldContain(x => x.Code == "TR");
        result.ShouldAllBe(x => !string.IsNullOrEmpty(x.Name));
    }

    [Fact]
    public async Task GetProvinces_ShouldReturnOnlySelectedCountry()
    {
        var result = await _locationAppService.GetProvincesAsync(InternshipJournalSeedIds.Countries.Turkey);

        result.ShouldNotBeEmpty();
        result.ShouldContain(x => x.Name == "İstanbul");

        var otherCountryResult = await _locationAppService.GetProvincesAsync(InternshipJournalSeedIds.Countries.Germany);
        otherCountryResult.ShouldNotContain(x => x.Name == "İstanbul");
    }

    [Fact]
    public async Task GetDistricts_ShouldReturnOnlySelectedProvince()
    {
        var result = await _locationAppService.GetDistrictsAsync(InternshipJournalSeedIds.Provinces.Istanbul);

        result.ShouldNotBeEmpty();
        result.ShouldContain(x => x.Name == "Kadıköy");

        var otherProvinceResult = await _locationAppService.GetDistrictsAsync(InternshipJournalSeedIds.Provinces.Ankara);
        otherProvinceResult.ShouldNotContain(x => x.Name == "Kadıköy");
    }

    [Fact]
    public async Task InactiveLocation_ShouldNotBeReturned()
    {
        var inactiveCountryId = Guid.NewGuid();
        var inactiveProvinceId = Guid.NewGuid();
        var inactiveDistrictId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var inactiveCountry = new Country(inactiveCountryId, "XX", "Test Ülkesi Pasif");
            inactiveCountry.Deactivate();
            await _countryRepository.InsertAsync(inactiveCountry);

            var inactiveProvince = new Province(inactiveProvinceId, InternshipJournalSeedIds.Countries.Turkey, "Test İli Pasif");
            inactiveProvince.Deactivate();
            await _provinceRepository.InsertAsync(inactiveProvince);

            var inactiveDistrict = new District(inactiveDistrictId, InternshipJournalSeedIds.Provinces.Istanbul, "Test İlçesi Pasif");
            inactiveDistrict.Deactivate();
            await _districtRepository.InsertAsync(inactiveDistrict);
        });

        var countries = await _locationAppService.GetCountriesAsync();
        countries.ShouldNotContain(x => x.Id == inactiveCountryId);

        var provinces = await _locationAppService.GetProvincesAsync(InternshipJournalSeedIds.Countries.Turkey);
        provinces.ShouldNotContain(x => x.Id == inactiveProvinceId);

        var districts = await _locationAppService.GetDistrictsAsync(InternshipJournalSeedIds.Provinces.Istanbul);
        districts.ShouldNotContain(x => x.Id == inactiveDistrictId);
    }
}
