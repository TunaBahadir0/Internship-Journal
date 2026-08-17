using Volo.Abp.Modularity;

namespace InternshipJournal;

[DependsOn(
    typeof(InternshipJournalDomainModule),
    typeof(InternshipJournalTestBaseModule)
)]
public class InternshipJournalDomainTestModule : AbpModule
{

}
