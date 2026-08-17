using InternshipJournal.Locations;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreLocationAppServiceTests : LocationAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
