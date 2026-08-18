using System;
using System.Threading.Tasks;
using InternshipJournal.Data;
using InternshipJournal.Locations;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace InternshipJournal.Workplaces;

public abstract class WorkplaceAppServiceTests<TStartupModule> : InternshipJournalApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IWorkplaceAppService _workplaceAppService;
    private readonly IRepository<District, Guid> _districtRepository;

    protected WorkplaceAppServiceTests()
    {
        _workplaceAppService = GetRequiredService<IWorkplaceAppService>();
        _districtRepository = GetRequiredService<IRepository<District, Guid>>();
    }

    [Fact]
    public async Task Create_WhenDistrictInactive_ShouldFail()
    {
        var inactiveDistrictId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var inactiveDistrict = new District(inactiveDistrictId, InternshipJournalSeedIds.Provinces.Istanbul, "Test İlçesi Pasif");
            inactiveDistrict.Deactivate();
            await _districtRepository.InsertAsync(inactiveDistrict);
        });

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
            {
                Name = "Pasif İlçe Testi",
                DistrictId = inactiveDistrictId,
                AddressLine = "Test Mahallesi No: 1"
            });
        });
    }

    [Fact]
    public async Task Create_WhenDistrictDoesNotExist_ShouldFail()
    {
        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
            {
                Name = "Olmayan İlçe Testi",
                DistrictId = Guid.NewGuid(),
                AddressLine = "Test Mahallesi No: 1"
            });
        });
    }

    [Fact]
    public async Task Create_WhenNameAlreadyExists_ShouldFail()
    {
        await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
        {
            Name = "Tekrarlanan Şirket Adı",
            DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
            AddressLine = "Test Mahallesi No: 1"
        });

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
            {
                Name = "Tekrarlanan Şirket Adı",
                DistrictId = InternshipJournalSeedIds.Districts.Uskudar,
                AddressLine = "Başka Mahalle No: 2"
            });
        });
    }

    [Fact]
    public async Task ChangeAddress_WhenDistrictValid_ShouldChange()
    {
        var created = await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
        {
            Name = "Adres Değişikliği Şirketi",
            DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
            AddressLine = "Eski Mahalle No: 1"
        });

        var updated = await _workplaceAppService.UpdateAsync(created.Id, new UpdateWorkplaceDto
        {
            Name = created.Name,
            DistrictId = InternshipJournalSeedIds.Districts.Uskudar,
            AddressLine = "Yeni Mahalle No: 2"
        });

        updated.DistrictId.ShouldBe(InternshipJournalSeedIds.Districts.Uskudar);
        updated.DistrictName.ShouldBe("Üsküdar");
        updated.AddressLine.ShouldBe("Yeni Mahalle No: 2");
    }

    [Fact]
    public async Task ChangeCoordinates_WhenLatitudeInvalid_ShouldFail()
    {
        var created = await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
        {
            Name = "Koordinat Testi Şirketi",
            DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
            AddressLine = "Test Mahallesi No: 1"
        });

        await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _workplaceAppService.UpdateAsync(created.Id, new UpdateWorkplaceDto
            {
                Name = created.Name,
                DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
                AddressLine = created.AddressLine,
                Latitude = 150
            });
        });
    }

    [Fact]
    public async Task GetDetail_ShouldReturnCountryProvinceDistrictNames()
    {
        var created = await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
        {
            Name = "Detay Testi Şirketi",
            DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
            AddressLine = "Test Mahallesi No: 1"
        });

        var detail = await _workplaceAppService.GetAsync(created.Id);

        detail.DistrictName.ShouldBe("Kadıköy");
        detail.ProvinceName.ShouldBe("İstanbul");
        detail.CountryName.ShouldBe("Türkiye");
    }

    [Fact]
    public async Task Deactivate_ShouldSetWorkplaceInactive()
    {
        var created = await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
        {
            Name = "Pasifleştirme Testi Şirketi",
            DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
            AddressLine = "Test Mahallesi No: 1"
        });

        await _workplaceAppService.DeactivateAsync(created.Id);

        var detail = await _workplaceAppService.GetAsync(created.Id);
        detail.IsActive.ShouldBeFalse();
    }
}
