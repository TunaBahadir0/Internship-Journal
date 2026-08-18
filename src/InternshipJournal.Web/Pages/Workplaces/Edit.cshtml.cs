using System;
using System.Threading.Tasks;
using InternshipJournal.Workplaces;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace InternshipJournal.Web.Pages.Workplaces;

public class EditModel : InternshipJournalPageModel
{
    private readonly IWorkplaceAppService _workplaceAppService;

    public EditModel(IWorkplaceAppService workplaceAppService)
    {
        _workplaceAppService = workplaceAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateWorkplaceDto Workplace { get; set; } = new();

    public Guid SelectedCountryId { get; set; }

    public Guid SelectedProvinceId { get; set; }

    public async Task OnGetAsync()
    {
        var detail = await _workplaceAppService.GetAsync(Id);

        Workplace = new UpdateWorkplaceDto
        {
            Name = detail.Name,
            TaxNumber = detail.TaxNumber,
            Phone = detail.Phone,
            Email = detail.Email,
            Website = detail.Website,
            DistrictId = detail.DistrictId,
            AddressLine = detail.AddressLine,
            PostalCode = detail.PostalCode,
            Latitude = detail.Latitude,
            Longitude = detail.Longitude
        };

        SelectedCountryId = detail.CountryId;
        SelectedProvinceId = detail.ProvinceId;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _workplaceAppService.UpdateAsync(Id, Workplace);
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
