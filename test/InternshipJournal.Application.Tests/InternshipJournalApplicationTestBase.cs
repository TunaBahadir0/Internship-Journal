using Volo.Abp.Modularity;

namespace InternshipJournal;

public abstract class InternshipJournalApplicationTestBase<TStartupModule> : InternshipJournalTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
