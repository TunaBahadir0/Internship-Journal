using System;
using System.Threading.Tasks;
using InternshipJournal.Locations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace InternshipJournal.Workplaces;

public class WorkplaceManager : DomainService
{
    private readonly IWorkplaceRepository _workplaceRepository;
    private readonly IRepository<District, Guid> _districtRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IRepository<Country, Guid> _countryRepository;

    public WorkplaceManager(
        IWorkplaceRepository workplaceRepository,
        IRepository<District, Guid> districtRepository,
        IRepository<Province, Guid> provinceRepository,
        IRepository<Country, Guid> countryRepository)
    {
        _workplaceRepository = workplaceRepository;
        _districtRepository = districtRepository;
        _provinceRepository = provinceRepository;
        _countryRepository = countryRepository;
    }

    public async Task<Workplace> CreateAsync(
        string name,
        Guid districtId,
        string addressLine,
        string? postalCode,
        string? taxNumber,
        string? phone,
        string? email,
        string? website,
        decimal? latitude,
        decimal? longitude)
    {
        await ValidateDistrictAsync(districtId);
        await ValidateNameAsync(name);

        return new Workplace(
            GuidGenerator.Create(),
            name,
            districtId,
            addressLine,
            postalCode,
            taxNumber,
            phone,
            email,
            website,
            latitude,
            longitude);
    }

    public async Task ChangeNameAsync(Workplace workplace, string name)
    {
        await ValidateNameAsync(name, workplace.Id);
        workplace.Rename(name);
    }

    public async Task ChangeAddressAsync(Workplace workplace, Guid districtId, string addressLine, string? postalCode)
    {
        await ValidateDistrictAsync(districtId);
        workplace.ChangeAddress(districtId, addressLine, postalCode);
    }

    private async Task ValidateNameAsync(string name, Guid? excludedId = null)
    {
        if (await _workplaceRepository.IsNameInUseAsync(name, excludedId))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.WorkplaceNameAlreadyExists);
        }
    }

    private async Task ValidateDistrictAsync(Guid districtId)
    {
        var district = await _districtRepository.FindAsync(districtId);
        if (district == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.WorkplaceDistrictNotFound);
        }

        if (!district.IsActive)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InactiveLocationCannotBeSelected);
        }

        var province = await _provinceRepository.GetAsync(district.ProvinceId);
        if (!province.IsActive)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InactiveLocationCannotBeSelected);
        }

        var country = await _countryRepository.GetAsync(province.CountryId);
        if (!country.IsActive)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InactiveLocationCannotBeSelected);
        }
    }
}
