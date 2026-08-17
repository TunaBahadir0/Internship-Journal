using InternshipJournal.Samples;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Domains;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
