using InternshipJournal.Skills;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreSkillAppServiceTests : SkillAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
