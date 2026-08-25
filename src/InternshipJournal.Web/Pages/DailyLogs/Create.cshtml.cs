using System;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace InternshipJournal.Web.Pages.DailyLogs;

[Authorize]
public class CreateModel : InternshipJournalPageModel
{
    private readonly IDailyLogAppService _dailyLogAppService;

    public CreateModel(IDailyLogAppService dailyLogAppService)
    {
        _dailyLogAppService = dailyLogAppService;
    }

    [BindProperty]
    public CreateDailyLogDto DailyLog { get; set; } = new() { LogDate = DateTime.Today };

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        DailyLogDetailDto created;
        try
        {
            created = await _dailyLogAppService.CreateAsync(DailyLog);
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("./Detail", new { id = created.Id });
    }
}
