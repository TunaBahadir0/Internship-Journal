using InternshipJournal.Locations;
using InternshipJournal.Workplaces;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace InternshipJournal;

[Mapper]
public partial class InternshipJournalApplicationMappers
{
    /* You can configure your Mapperly mapping configuration here.
     * Alternatively, you can split your mapping configurations
     * into multiple mapper classes for a better organization. */

    public partial CountryLookupDto Map(Country source);

    public partial ProvinceLookupDto Map(Province source);

    public partial DistrictLookupDto Map(District source);

    public partial WorkplaceDto MapToWorkplaceDto(WorkplaceWithLocation source);

    public partial WorkplaceDetailDto MapToWorkplaceDetailDto(WorkplaceWithLocation source);
}
