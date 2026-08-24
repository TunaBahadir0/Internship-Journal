using InternshipJournal.DailyLogs;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreDailyLogAppServiceTests : DailyLogAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
