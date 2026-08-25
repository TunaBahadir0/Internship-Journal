using InternshipJournal.MentorReviews;
using Xunit;

namespace InternshipJournal.EntityFrameworkCore.Applications;

[Collection(InternshipJournalTestConsts.CollectionDefinitionName)]
public class EfCoreMentorReviewAppServiceTests : MentorReviewAppServiceTests<InternshipJournalEntityFrameworkCoreTestModule>
{

}
