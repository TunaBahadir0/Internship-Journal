using InternshipJournal.Workplaces;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreWorkplaceAppServiceTests : WorkplaceAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
