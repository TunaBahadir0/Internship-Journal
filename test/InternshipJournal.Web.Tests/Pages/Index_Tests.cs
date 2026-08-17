using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace InternshipJournal.Pages;

public class Index_Tests : InternshipJournalWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
