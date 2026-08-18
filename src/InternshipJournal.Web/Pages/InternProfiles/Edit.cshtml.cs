using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.InternProfiles;
using InternshipJournal.Workplaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp;
using Volo.Abp.Identity;

namespace InternshipJournal.Web.Pages.InternProfiles;

[Authorize]
public class EditModel : InternshipJournalPageModel
{
    private readonly IInternProfileAppService _internProfileAppService;
    private readonly IIdentityUserAppService _identityUserAppService;
    private readonly IWorkplaceAppService _workplaceAppService;

    public EditModel(
        IInternProfileAppService internProfileAppService,
        IIdentityUserAppService identityUserAppService,
        IWorkplaceAppService workplaceAppService)
    {
        _internProfileAppService = internProfileAppService;
        _identityUserAppService = identityUserAppService;
        _workplaceAppService = workplaceAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateInternProfileDto InternProfile { get; set; } = new();

    public List<SelectListItem> Users { get; set; } = new();

    public List<SelectListItem> Workplaces { get; set; } = new();

    public async Task OnGetAsync()
    {
        var detail = await _internProfileAppService.GetAsync(Id);

        InternProfile = new UpdateInternProfileDto
        {
            MentorUserId = detail.MentorUserId,
            WorkplaceId = detail.WorkplaceId,
            University = detail.University,
            SchoolDepartment = detail.SchoolDepartment,
            StudentNumber = detail.StudentNumber,
            InternshipStartDate = detail.InternshipStartDate,
            InternshipEndDate = detail.InternshipEndDate,
            RequiredWorkDays = detail.RequiredWorkDays
        };

        await PopulateSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync();
            return Page();
        }

        try
        {
            await _internProfileAppService.UpdateAsync(Id, InternProfile);
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateSelectListsAsync();
            return Page();
        }

        return RedirectToPage("./Index");
    }

    private async Task PopulateSelectListsAsync()
    {
        var users = await _identityUserAppService.GetListAsync(new GetIdentityUsersInput { MaxResultCount = 1000 });
        Users = users.Items.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();

        var workplaces = await _workplaceAppService.GetListAsync(new GetWorkplaceListInput { IsActive = true, MaxResultCount = 1000 });
        Workplaces = workplaces.Items.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
    }
}
