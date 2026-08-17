using Volo.Abp.Modularity;

namespace InternshipJournal;

/* Inherit from this class for your domain layer tests. */
public abstract class InternshipJournalDomainTestBase<TStartupModule> : InternshipJournalTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
