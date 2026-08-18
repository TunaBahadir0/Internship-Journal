using System;
using System.Threading.Tasks;
using InternshipJournal.Workplaces;
using Microsoft.AspNetCore.Mvc;

namespace InternshipJournal.Web.Pages.Workplaces;

public class DetailModel : InternshipJournalPageModel
{
    private readonly IWorkplaceAppService _workplaceAppService;

    public DetailModel(IWorkplaceAppService workplaceAppService)
    {
        _workplaceAppService = workplaceAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public WorkplaceDetailDto Workplace { get; set; } = null!;

    public async Task OnGetAsync()
    {
        Workplace = await _workplaceAppService.GetAsync(Id);
    }
}
