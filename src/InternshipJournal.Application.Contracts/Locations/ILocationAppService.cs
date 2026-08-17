using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace InternshipJournal.Locations;

public interface ILocationAppService : IApplicationService
{
    Task<List<CountryLookupDto>> GetCountriesAsync();

    Task<List<ProvinceLookupDto>> GetProvincesAsync(Guid countryId);

    Task<List<DistrictLookupDto>> GetDistrictsAsync(Guid provinceId);
}
