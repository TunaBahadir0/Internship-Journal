using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using InternshipJournal.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp;

namespace InternshipJournal.Web.Pages.DailyLogs;

[Authorize]
public class IndexModel : InternshipJournalPageModel
{
    private readonly IDailyLogAppService _dailyLogAppService;

    public IndexModel(IDailyLogAppService dailyLogAppService)
    {
        _dailyLogAppService = dailyLogAppService;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DailyLogStatus? Status { get; set; }

    public List<DailyLogDto> DailyLogs { get; set; } = new();

    public List<SelectListItem> StatusOptions { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        StatusOptions = Enum.GetValues<DailyLogStatus>()
            .Select(x => new SelectListItem(L["Enum:DailyLogStatus:" + x].ToString(), x.ToString()))
            .ToList();

        try
        {
            var result = await _dailyLogAppService.GetListAsync(new GetDailyLogListInput
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Status = Status,
                MaxResultCount = 1000
            });

            DailyLogs = result.Items.ToList();
        }
        catch (BusinessException ex)
        {
            ErrorMessage = GetErrorMessage(ex);
        }
    }
}
