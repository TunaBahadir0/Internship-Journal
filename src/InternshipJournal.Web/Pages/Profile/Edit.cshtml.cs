using System;
using System.Threading.Tasks;
using InternshipJournal.InternProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace InternshipJournal.Web.Pages.Profile;

[Authorize]
public class EditModel : InternshipJournalPageModel
{
    private readonly IInternProfileAppService _internProfileAppService;

    public EditModel(IInternProfileAppService internProfileAppService)
    {
        _internProfileAppService = internProfileAppService;
    }

    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateInternProfileDto InternProfile { get; set; } = new();

    public async Task OnGetAsync()
    {
        var detail = await _internProfileAppService.GetMyProfileAsync();

        Id = detail.Id;
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
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _internProfileAppService.UpdateAsync(Id, InternProfile);
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
