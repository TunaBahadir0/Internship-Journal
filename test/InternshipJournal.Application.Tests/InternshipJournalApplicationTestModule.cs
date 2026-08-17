using Volo.Abp.Modularity;

namespace InternshipJournal;

[DependsOn(
    typeof(InternshipJournalApplicationModule),
    typeof(InternshipJournalDomainTestModule)
)]
public class InternshipJournalApplicationTestModule : AbpModule
{

}
