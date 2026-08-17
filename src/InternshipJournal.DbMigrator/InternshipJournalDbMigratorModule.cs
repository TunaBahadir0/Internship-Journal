using InternshipJournal.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace InternshipJournal.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(InternshipJournalEntityFrameworkCoreModule),
    typeof(InternshipJournalApplicationContractsModule)
    )]
public class InternshipJournalDbMigratorModule : AbpModule
{
}
