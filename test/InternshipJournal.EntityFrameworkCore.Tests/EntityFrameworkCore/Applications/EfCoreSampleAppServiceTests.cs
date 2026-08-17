using InternshipJournal.Samples;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
