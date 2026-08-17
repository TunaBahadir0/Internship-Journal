using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace InternshipJournal.Locations;

public class LocationAppService : InternshipJournalAppService, ILocationAppService
{
    private readonly IRepository<Country, Guid> _countryRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IRepository<District, Guid> _districtRepository;
    private readonly InternshipJournalApplicationMappers _mapper;

    public LocationAppService(
        IRepository<Country, Guid> countryRepository,
        IRepository<Province, Guid> provinceRepository,
        IRepository<District, Guid> districtRepository,
        InternshipJournalApplicationMappers mapper)
    {
        _countryRepository = countryRepository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _mapper = mapper;
    }

    public async Task<List<CountryLookupDto>> GetCountriesAsync()
    {
        var countries = await _countryRepository.GetListAsync(x => x.IsActive);

        return countries
            .OrderBy(x => x.Name)
            .Select(x => _mapper.Map(x))
            .ToList();
    }

    public async Task<List<ProvinceLookupDto>> GetProvincesAsync(Guid countryId)
    {
        var provinces = await _provinceRepository.GetListAsync(x => x.IsActive && x.CountryId == countryId);

        return provinces
            .OrderBy(x => x.Name)
            .Select(x => _mapper.Map(x))
            .ToList();
    }

    public async Task<List<DistrictLookupDto>> GetDistrictsAsync(Guid provinceId)
    {
        var districts = await _districtRepository.GetListAsync(x => x.IsActive && x.ProvinceId == provinceId);

        return districts
            .OrderBy(x => x.Name)
            .Select(x => _mapper.Map(x))
            .ToList();
    }
}
