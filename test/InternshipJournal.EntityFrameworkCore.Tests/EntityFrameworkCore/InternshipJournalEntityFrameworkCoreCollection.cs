using Xunit;

namespace InternshipJournal.EntityFrameworkCore;

[CollectionDefinition(InternshipJournalTestConsts.CollectionDefinitionName)]
public class InternshipJournalEntityFrameworkCoreCollection : ICollectionFixture<InternshipJournalEntityFrameworkCoreFixture>
{

}
