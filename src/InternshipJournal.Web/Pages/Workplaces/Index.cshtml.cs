using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.Workplaces;
using Microsoft.AspNetCore.Mvc;

namespace InternshipJournal.Web.Pages.Workplaces;

public class IndexModel : InternshipJournalPageModel
{
    private readonly IWorkplaceAppService _workplaceAppService;

    public IndexModel(IWorkplaceAppService workplaceAppService)
    {
        _workplaceAppService = workplaceAppService;
    }

    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    public List<WorkplaceDto> Workplaces { get; set; } = new();

    public async Task OnGetAsync()
    {
        var result = await _workplaceAppService.GetListAsync(new GetWorkplaceListInput
        {
            Filter = Filter,
            MaxResultCount = 1000
        });

        Workplaces = result.Items.ToList();
    }

    public async Task<IActionResult> OnPostActivateAsync(Guid id)
    {
        await _workplaceAppService.ActivateAsync(id);
        return RedirectToPage(new { Filter });
    }

    public async Task<IActionResult> OnPostDeactivateAsync(Guid id)
    {
        await _workplaceAppService.DeactivateAsync(id);
        return RedirectToPage(new { Filter });
    }
}
