using System.Threading.Tasks;
using InternshipJournal.InternProfiles;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Entities;

namespace InternshipJournal.Web.Pages.Profile;

[Authorize]
public class IndexModel : InternshipJournalPageModel
{
    private readonly IInternProfileAppService _internProfileAppService;

    public IndexModel(IInternProfileAppService internProfileAppService)
    {
        _internProfileAppService = internProfileAppService;
    }

    public InternProfileDetailDto? InternProfile { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            InternProfile = await _internProfileAppService.GetMyProfileAsync();
        }
        catch (EntityNotFoundException)
        {
            InternProfile = null;
        }
    }
}
