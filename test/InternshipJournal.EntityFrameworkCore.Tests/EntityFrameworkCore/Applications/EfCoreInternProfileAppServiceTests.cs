using InternshipJournal.InternProfiles;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreInternProfileAppServiceTests : InternProfileAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
