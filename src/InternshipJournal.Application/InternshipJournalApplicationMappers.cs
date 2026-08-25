using InternshipJournal.DailyLogs;
using InternshipJournal.InternProfiles;
using InternshipJournal.Locations;
using InternshipJournal.Skills;
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

    public partial InternProfileDto MapToInternProfileDto(InternProfileWithDetails source);

    public partial InternProfileDetailDto MapToInternProfileDetailDto(InternProfileWithDetails source);

    public partial DailyLogDto MapToDailyLogDto(DailyLog source);

    public partial DailyLogDetailDto MapToDailyLogDetailDto(DailyLog source);

    public partial DailyLogItemDto Map(DailyLogItem source);

    public partial DailyLogSkillDto Map(DailyLogSkill source);

    public partial ProblemSolvingEntryDto Map(ProblemSolvingEntry source);

    public partial SkillLookupDto Map(Skill source);
}
