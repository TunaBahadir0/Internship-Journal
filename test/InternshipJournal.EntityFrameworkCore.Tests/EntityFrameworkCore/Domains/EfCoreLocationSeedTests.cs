using InternshipJournal.Locations;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Domains;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreLocationSeedTests : LocationSeedTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
