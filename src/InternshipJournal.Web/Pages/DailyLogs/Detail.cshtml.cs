using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using InternshipJournal.Enums;
using InternshipJournal.MentorReviews;
using InternshipJournal.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp;

namespace InternshipJournal.Web.Pages.DailyLogs;

[Authorize]
public class DetailModel : InternshipJournalPageModel
{
    private readonly IDailyLogAppService _dailyLogAppService;
    private readonly IMentorReviewAppService _mentorReviewAppService;
    private readonly ISkillAppService _skillAppService;

    public DetailModel(
        IDailyLogAppService dailyLogAppService,
        IMentorReviewAppService mentorReviewAppService,
        ISkillAppService skillAppService)
    {
        _dailyLogAppService = dailyLogAppService;
        _mentorReviewAppService = mentorReviewAppService;
        _skillAppService = skillAppService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public DailyLogDetailDto DailyLog { get; set; } = null!;

    [BindProperty]
    public UpdateDailyLogSummaryDto SummaryInput { get; set; } = new();

    [BindProperty]
    public AddDailyLogItemInput NewItem { get; set; } = new();

    [BindProperty]
    public AddDailyLogSkillInput NewSkill { get; set; } = new();

    [BindProperty]
    public AddProblemSolvingEntryInput NewProblem { get; set; } = new();

    [BindProperty]
    public ApproveDailyLogReviewInput ApproveInput { get; set; } = new();

    [BindProperty]
    public RequestDailyLogRevisionInput RevisionInput { get; set; } = new();

    public List<SelectListItem> WorkTypeOptions { get; set; } = new();

    public List<SelectListItem> LearningLevelOptions { get; set; } = new();

    public List<SelectListItem> SkillOptions { get; set; } = new();

    public List<MentorReviewDto> Reviews { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public bool IsEditable => DailyLog.Status is DailyLogStatus.Draft or DailyLogStatus.RevisionRequested;

    public string SkillName(Guid skillId)
    {
        return SkillOptions.FirstOrDefault(x => x.Value == skillId.ToString())?.Text ?? skillId.ToString();
    }

    public async Task OnGetAsync()
    {
        if (TempData.TryGetValue("DailyLogError", out var error) && error is string message)
        {
            ErrorMessage = message;
        }

        await LoadAsync();
        SummaryInput.Summary = DailyLog.Summary;
    }

    public async Task<IActionResult> OnPostUpdateSummaryAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(SummaryInput, nameof(SummaryInput)))
        {
            await LoadAsync();
            return Page();
        }

        return await ExecuteAsync(() => _dailyLogAppService.UpdateSummaryAsync(Id, SummaryInput));
    }

    public async Task<IActionResult> OnPostAddItemAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(NewItem, nameof(NewItem)))
        {
            await LoadAsync();
            return Page();
        }

        return await ExecuteAsync(() => _dailyLogAppService.AddItemAsync(Id, NewItem));
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(Guid itemId)
    {
        return await ExecuteAsync(() => _dailyLogAppService.RemoveItemAsync(Id, itemId));
    }

    public async Task<IActionResult> OnPostAddSkillAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(NewSkill, nameof(NewSkill)))
        {
            await LoadAsync();
            return Page();
        }

        return await ExecuteAsync(() => _dailyLogAppService.AddSkillAsync(Id, NewSkill));
    }

    public async Task<IActionResult> OnPostRemoveSkillAsync(Guid skillEntryId)
    {
        return await ExecuteAsync(() => _dailyLogAppService.RemoveSkillAsync(Id, skillEntryId));
    }

    public async Task<IActionResult> OnPostAddProblemAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(NewProblem, nameof(NewProblem)))
        {
            await LoadAsync();
            return Page();
        }

        return await ExecuteAsync(() => _dailyLogAppService.AddProblemAsync(Id, NewProblem));
    }

    public async Task<IActionResult> OnPostRemoveProblemAsync(Guid problemId)
    {
        return await ExecuteAsync(() => _dailyLogAppService.RemoveProblemAsync(Id, problemId));
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        return await ExecuteAsync(() => _dailyLogAppService.SubmitAsync(Id));
    }

    public async Task<IActionResult> OnPostRequestRevisionAsync()
    {
        ModelState.Clear();
        if (!TryValidateModel(RevisionInput, nameof(RevisionInput)))
        {
            await LoadAsync();
            return Page();
        }

        return await ExecuteReviewAsync(() => _mentorReviewAppService.RequestRevisionAsync(Id, RevisionInput));
    }

    public async Task<IActionResult> OnPostApproveAsync()
    {
        return await ExecuteReviewAsync(() => _mentorReviewAppService.ApproveAsync(Id, ApproveInput));
    }

    public async Task<IActionResult> OnPostReturnToDraftAsync()
    {
        return await ExecuteAsync(() => _dailyLogAppService.ReturnToDraftAsync(Id));
    }

    private async Task<IActionResult> ExecuteAsync(Func<Task<DailyLogDetailDto>> action)
    {
        try
        {
            await action();
        }
        catch (BusinessException ex)
        {
            TempData["DailyLogError"] = GetErrorMessage(ex);
        }

        return RedirectToPage(new { id = Id });
    }

    private async Task<IActionResult> ExecuteReviewAsync(Func<Task<MentorReviewDto>> action)
    {
        try
        {
            await action();
        }
        catch (BusinessException ex)
        {
            TempData["DailyLogError"] = GetErrorMessage(ex);
        }

        return RedirectToPage(new { id = Id });
    }

    private async Task LoadAsync()
    {
        DailyLog = await _dailyLogAppService.GetAsync(Id);

        WorkTypeOptions = Enum.GetValues<WorkType>()
            .Select(x => new SelectListItem(L["Enum:WorkType:" + x].ToString(), x.ToString()))
            .ToList();

        LearningLevelOptions = Enum.GetValues<LearningLevel>()
            .Select(x => new SelectListItem(L["Enum:LearningLevel:" + x].ToString(), x.ToString()))
            .ToList();

        var skills = await _skillAppService.GetListAsync();
        SkillOptions = skills.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

        Reviews = await _mentorReviewAppService.GetListByDailyLogAsync(Id);
    }
}
