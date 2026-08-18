using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternshipJournal.Web.Pages.InternProfiles;

[Authorize]
public class IndexModel : InternshipJournalPageModel
{
    private readonly IInternProfileAppService _internProfileAppService;

    public IndexModel(IInternProfileAppService internProfileAppService)
    {
        _internProfileAppService = internProfileAppService;
    }

    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    [BindProperty(SupportsGet = true)]
    public InternshipStatus? Status { get; set; }

    public List<InternProfileDto> InternProfiles { get; set; } = new();

    public async Task OnGetAsync()
    {
        var result = await _internProfileAppService.GetListAsync(new GetInternProfileListInput
        {
            Filter = Filter,
            Status = Status,
            MaxResultCount = 1000
        });

        InternProfiles = result.Items.ToList();
    }

    public async Task<IActionResult> OnPostStartAsync(Guid id)
    {
        await _internProfileAppService.StartAsync(id);
        return RedirectToPage(new { Filter, Status });
    }

    public async Task<IActionResult> OnPostCompleteAsync(Guid id)
    {
        await _internProfileAppService.CompleteAsync(id);
        return RedirectToPage(new { Filter, Status });
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id)
    {
        await _internProfileAppService.CancelAsync(id);
        return RedirectToPage(new { Filter, Status });
    }
}
