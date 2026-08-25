using System.Collections.Generic;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using Microsoft.AspNetCore.Authorization;

namespace InternshipJournal.Web.Pages.MentorReviews;

[Authorize]
public class IndexModel : InternshipJournalPageModel
{
    private readonly IDailyLogAppService _dailyLogAppService;

    public IndexModel(IDailyLogAppService dailyLogAppService)
    {
        _dailyLogAppService = dailyLogAppService;
    }

    public List<DailyLogForReviewDto> PendingLogs { get; set; } = new();

    public async Task OnGetAsync()
    {
        PendingLogs = await _dailyLogAppService.GetListForReviewAsync();
    }
}
