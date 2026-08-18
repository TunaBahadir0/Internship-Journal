using System.Threading.Tasks;
using InternshipJournal.Workplaces;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace InternshipJournal.Web.Pages.Workplaces;

public class CreateModel : InternshipJournalPageModel
{
    private readonly IWorkplaceAppService _workplaceAppService;

    public CreateModel(IWorkplaceAppService workplaceAppService)
    {
        _workplaceAppService = workplaceAppService;
    }

    [BindProperty]
    public CreateWorkplaceDto Workplace { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _workplaceAppService.CreateAsync(Workplace);
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
