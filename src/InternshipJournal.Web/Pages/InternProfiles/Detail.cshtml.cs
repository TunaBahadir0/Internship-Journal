using System;
using System.Threading.Tasks;
using InternshipJournal.InternProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternshipJournal.Web.Pages.InternProfiles;

[Authorize]
public class DetailModel : InternshipJournalPageModel
{
    private readonly IInternProfileAppService _internProfileAppService;

    public DetailModel(IInternProfileAppService internProfileAppService)
    {
        _internProfileAppService = internProfileAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public InternProfileDetailDto InternProfile { get; set; } = null!;

    public async Task OnGetAsync()
    {
        InternProfile = await _internProfileAppService.GetAsync(Id);
    }
}
