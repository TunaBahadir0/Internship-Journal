using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InternshipJournal.Data;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using InternshipJournal.MentorReviews;
using InternshipJournal.Workplaces;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Xunit;

namespace InternshipJournal.DailyLogs;

public abstract class DailyLogAppServiceTests<TStartupModule> : InternshipJournalApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDailyLogAppService _dailyLogAppService;
    private readonly IInternProfileAppService _internProfileAppService;
    private readonly IWorkplaceAppService _workplaceAppService;
    private readonly IMentorReviewAppService _mentorReviewAppService;
    private readonly IdentityUserManager _identityUserManager;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    protected DailyLogAppServiceTests()
    {
        _dailyLogAppService = GetRequiredService<IDailyLogAppService>();
        _internProfileAppService = GetRequiredService<IInternProfileAppService>();
        _workplaceAppService = GetRequiredService<IWorkplaceAppService>();
        _mentorReviewAppService = GetRequiredService<IMentorReviewAppService>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
    }

    [Fact]
    public async Task Create_WithValidDate_ShouldCreate()
    {
        var (userId, _, _) = await CreateActiveInternAsync("create.valid");

        using (LoginAs(userId))
        {
            var result = await _dailyLogAppService.CreateAsync(new CreateDailyLogDto
            {
                LogDate = DateTime.Today,
                Summary = "İlk gün"
            });

            result.LogDate.ShouldBe(DateTime.Today);
            result.Status.ShouldBe(DailyLogStatus.Draft);
        }
    }

    [Fact]
    public async Task Create_WhenDailyLogExists_ShouldFail()
    {
        var (userId, _, _) = await CreateActiveInternAsync("create.duplicate");

        using (LoginAs(userId))
        {
            await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });

            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
            });
        }
    }

    [Fact]
    public async Task Create_WhenProfileInactive_ShouldFail()
    {
        var userId = await CreateUserAsync("intern.create.inactive");
        var mentorId = await CreateUserAsync("mentor.create.inactive");
        var workplaceId = await CreateWorkplaceAsync("Pasif Profil Şirketi");

        // Kasıtlı olarak StartAsync çağrılmıyor; profil Draft durumunda kalıyor.
        await _internProfileAppService.CreateAsync(BuildCreateProfileDto(userId, mentorId, workplaceId));

        using (LoginAs(userId))
        {
            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
            });
        }
    }

    [Fact]
    public async Task GetList_ShouldReturnOnlyCurrentInternLogs()
    {
        var (userId1, _, _) = await CreateActiveInternAsync("list.intern1");
        var (userId2, _, _) = await CreateActiveInternAsync("list.intern2");

        using (LoginAs(userId1))
        {
            await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
        }

        using (LoginAs(userId2))
        {
            await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
        }

        using (LoginAs(userId1))
        {
            var result = await _dailyLogAppService.GetListAsync(new GetDailyLogListInput());

            result.TotalCount.ShouldBe(1);
        }
    }

    [Fact]
    public async Task GetList_WhenDateFilterProvided_ShouldFilter()
    {
        var (userId, _, _) = await CreateActiveInternAsync("list.datefilter", startDate: DateTime.Today.AddDays(-10));

        using (LoginAs(userId))
        {
            await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today.AddDays(-5) });
            await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });

            var result = await _dailyLogAppService.GetListAsync(new GetDailyLogListInput
            {
                StartDate = DateTime.Today.AddDays(-1)
            });

            result.TotalCount.ShouldBe(1);
            result.Items.Single().LogDate.ShouldBe(DateTime.Today);
        }
    }

    [Fact]
    public async Task GetListForReview_ShouldReturnOnlySubmittedLogsOfOwnInterns()
    {
        var (userId, mentorId, _) = await CreateActiveInternAsync("review.list");
        var created = await CreateDraftLogAsync(userId);

        using (LoginAs(userId))
        {
            await _dailyLogAppService.AddItemAsync(created.Id, new AddDailyLogItemInput
            {
                Title = "Bir madde",
                WorkType = WorkType.Development,
                DurationMinutes = 30,
                IsCompleted = true
            });
            await _dailyLogAppService.SubmitAsync(created.Id);
        }

        using (LoginAs(mentorId))
        {
            var result = await _dailyLogAppService.GetListForReviewAsync();

            result.Count.ShouldBe(1);
            result.Single().InternProfileId.ShouldNotBe(Guid.Empty);
        }
    }

    [Fact]
    public async Task UpdateSummary_WhenLogEditable_ShouldUpdate()
    {
        var (userId, _, _) = await CreateActiveInternAsync("update.editable");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto updated;
        using (LoginAs(userId))
        {
            updated = await _dailyLogAppService.UpdateSummaryAsync(created.Id, new UpdateDailyLogSummaryDto
            {
                Summary = "Güncellenmiş özet"
            });
        }

        updated.Summary.ShouldBe("Güncellenmiş özet");
    }

    [Fact]
    public async Task UpdateSummary_WhenNotOwner_ShouldFail()
    {
        var (userId, _, _) = await CreateActiveInternAsync("update.notowner");
        var created = await CreateDraftLogAsync(userId);
        var otherUserId = await CreateUserAsync("intern.update.notowner.other");

        using (LoginAs(otherUserId))
        {
            await Assert.ThrowsAsync<BusinessException>(() =>
                _dailyLogAppService.UpdateSummaryAsync(created.Id, new UpdateDailyLogSummaryDto { Summary = "Başkasının günlüğü" }));
        }
    }

    [Fact]
    public async Task UpdateSummary_WhenLogApproved_ShouldFail()
    {
        var (userId, mentorId, _) = await CreateActiveInternAsync("update.approved");
        var created = await CreateDraftLogAsync(userId);

        await AddItemAsync(userId, created.Id, "Onay öncesi madde", 30);
        await SubmitAsync(userId, created.Id);
        await ApproveAsMentorAsync(mentorId, created.Id);

        using (LoginAs(userId))
        {
            await Assert.ThrowsAsync<BusinessException>(() =>
                _dailyLogAppService.UpdateSummaryAsync(created.Id, new UpdateDailyLogSummaryDto { Summary = "Değişmemeli" }));
        }
    }

    [Fact]
    public async Task GetDetail_ShouldReturnChildCollections()
    {
        var (userId, _, _) = await CreateActiveInternAsync("detail.children");
        var created = await CreateDraftLogAsync(userId);

        await AddItemAsync(userId, created.Id, "İlk madde", 60);

        DailyLogDetailDto detail;
        using (LoginAs(userId))
        {
            detail = await _dailyLogAppService.GetAsync(created.Id);
        }

        detail.Items.Count.ShouldBe(1);
        detail.TotalMinutes.ShouldBe(60);
    }

    [Fact]
    public async Task GetDetail_WhenMentorOfIntern_ShouldSucceed()
    {
        var (userId, mentorId, _) = await CreateActiveInternAsync("detail.mentorview");
        var created = await CreateDraftLogAsync(userId);

        using (LoginAs(mentorId))
        {
            var detail = await _dailyLogAppService.GetAsync(created.Id);
            detail.Id.ShouldBe(created.Id);
        }
    }

    [Fact]
    public async Task GetDetail_WhenUnrelatedUser_ShouldFail()
    {
        var (userId, _, _) = await CreateActiveInternAsync("detail.unrelated");
        var created = await CreateDraftLogAsync(userId);
        var strangerId = await CreateUserAsync("stranger.detail.unrelated");

        using (LoginAs(strangerId))
        {
            await Assert.ThrowsAsync<BusinessException>(() => _dailyLogAppService.GetAsync(created.Id));
        }
    }

    [Fact]
    public async Task AddItem_ShouldAddAndRecalculateTotal()
    {
        var (userId, _, _) = await CreateActiveInternAsync("item.add");
        var created = await CreateDraftLogAsync(userId);

        var result = await AddItemAsync(userId, created.Id, "Domain katmanı", 90);

        result.Items.Count.ShouldBe(1);
        result.TotalMinutes.ShouldBe(90);
    }

    [Fact]
    public async Task AddItem_WhenNotOwner_ShouldFail()
    {
        var (userId, mentorId, _) = await CreateActiveInternAsync("item.add.notowner");
        var created = await CreateDraftLogAsync(userId);

        using (LoginAs(mentorId))
        {
            await Assert.ThrowsAsync<BusinessException>(() =>
                _dailyLogAppService.AddItemAsync(created.Id, new AddDailyLogItemInput
                {
                    Title = "Mentor eklemeye çalışıyor",
                    WorkType = WorkType.Development,
                    DurationMinutes = 10,
                    IsCompleted = false
                }));
        }
    }

    [Fact]
    public async Task UpdateItem_ShouldUpdateAndRecalculateTotal()
    {
        var (userId, _, _) = await CreateActiveInternAsync("item.update");
        var created = await CreateDraftLogAsync(userId);
        var withItem = await AddItemAsync(userId, created.Id, "İlk hâli", 30);
        var itemId = withItem.Items.Single().Id;

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            result = await _dailyLogAppService.UpdateItemAsync(created.Id, itemId, new UpdateDailyLogItemInput
            {
                Title = "Güncellenmiş hâli",
                WorkType = WorkType.Testing,
                DurationMinutes = 45,
                IsCompleted = true
            });
        }

        result.Items.Single().Title.ShouldBe("Güncellenmiş hâli");
        result.TotalMinutes.ShouldBe(45);
    }

    [Fact]
    public async Task RemoveItem_ShouldRemoveAndRecalculateTotal()
    {
        var (userId, _, _) = await CreateActiveInternAsync("item.remove");
        var created = await CreateDraftLogAsync(userId);
        var withItem = await AddItemAsync(userId, created.Id, "Silinecek madde", 40);
        var itemId = withItem.Items.Single().Id;

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            result = await _dailyLogAppService.RemoveItemAsync(created.Id, itemId);
        }

        result.Items.ShouldBeEmpty();
        result.TotalMinutes.ShouldBe(0);
    }

    [Fact]
    public async Task AddSkill_ShouldAdd()
    {
        var (userId, _, _) = await CreateActiveInternAsync("skill.add");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            result = await _dailyLogAppService.AddSkillAsync(created.Id, new AddDailyLogSkillInput
            {
                SkillId = InternshipJournalSeedIds.Skills.CSharp,
                LearningLevel = LearningLevel.Practiced,
                Note = "İlk kez kullanıldı"
            });
        }

        result.Skills.Single().SkillId.ShouldBe(InternshipJournalSeedIds.Skills.CSharp);
    }

    [Fact]
    public async Task UpdateSkill_ShouldUpdate()
    {
        var (userId, _, _) = await CreateActiveInternAsync("skill.update");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            var withSkill = await _dailyLogAppService.AddSkillAsync(created.Id, new AddDailyLogSkillInput
            {
                SkillId = InternshipJournalSeedIds.Skills.CSharp,
                LearningLevel = LearningLevel.Introduced,
                Note = null
            });
            var skillEntryId = withSkill.Skills.Single().Id;

            result = await _dailyLogAppService.UpdateSkillAsync(created.Id, skillEntryId, new UpdateDailyLogSkillInput
            {
                LearningLevel = LearningLevel.Applied,
                Note = "İlerleme kaydedildi"
            });
        }

        result.Skills.Single().LearningLevel.ShouldBe(LearningLevel.Applied);
    }

    [Fact]
    public async Task RemoveSkill_ShouldRemove()
    {
        var (userId, _, _) = await CreateActiveInternAsync("skill.remove");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            var withSkill = await _dailyLogAppService.AddSkillAsync(created.Id, new AddDailyLogSkillInput
            {
                SkillId = InternshipJournalSeedIds.Skills.CSharp,
                LearningLevel = LearningLevel.Practiced,
                Note = null
            });
            var skillEntryId = withSkill.Skills.Single().Id;

            result = await _dailyLogAppService.RemoveSkillAsync(created.Id, skillEntryId);
        }

        result.Skills.ShouldBeEmpty();
    }

    [Fact]
    public async Task AddProblem_ShouldAdd()
    {
        var (userId, _, _) = await CreateActiveInternAsync("problem.add");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            result = await _dailyLogAppService.AddProblemAsync(created.Id, new AddProblemSolvingEntryInput
            {
                Title = "Migration hatası",
                ProblemDescription = "EF Core migration çalışmadı.",
                UsedArtificialIntelligence = false
            });
        }

        result.Problems.Single().Title.ShouldBe("Migration hatası");
    }

    [Fact]
    public async Task UpdateProblem_ShouldUpdate()
    {
        var (userId, _, _) = await CreateActiveInternAsync("problem.update");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            var withProblem = await _dailyLogAppService.AddProblemAsync(created.Id, new AddProblemSolvingEntryInput
            {
                Title = "İlk başlık",
                ProblemDescription = "İlk açıklama.",
                UsedArtificialIntelligence = false
            });
            var problemId = withProblem.Problems.Single().Id;

            result = await _dailyLogAppService.UpdateProblemAsync(created.Id, problemId, new UpdateProblemSolvingEntryInput
            {
                Title = "Güncellenmiş başlık",
                ProblemDescription = "Güncellenmiş açıklama.",
                UsedArtificialIntelligence = false
            });
        }

        result.Problems.Single().Title.ShouldBe("Güncellenmiş başlık");
    }

    [Fact]
    public async Task RemoveProblem_ShouldRemove()
    {
        var (userId, _, _) = await CreateActiveInternAsync("problem.remove");
        var created = await CreateDraftLogAsync(userId);

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            var withProblem = await _dailyLogAppService.AddProblemAsync(created.Id, new AddProblemSolvingEntryInput
            {
                Title = "Silinecek problem",
                ProblemDescription = "Açıklama.",
                UsedArtificialIntelligence = false
            });
            var problemId = withProblem.Problems.Single().Id;

            result = await _dailyLogAppService.RemoveProblemAsync(created.Id, problemId);
        }

        result.Problems.ShouldBeEmpty();
    }

    [Fact]
    public async Task Submit_WhenHasItem_ShouldSubmit()
    {
        var (userId, _, _) = await CreateActiveInternAsync("submit.valid");
        var created = await CreateDraftLogAsync(userId);
        await AddItemAsync(userId, created.Id, "Bir madde", 30);

        var result = await SubmitAsync(userId, created.Id);

        result.Status.ShouldBe(DailyLogStatus.Submitted);
        result.SubmittedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task Submit_WhenNoItem_ShouldFail()
    {
        var (userId, _, _) = await CreateActiveInternAsync("submit.noitem");
        var created = await CreateDraftLogAsync(userId);

        using (LoginAs(userId))
        {
            await Assert.ThrowsAsync<BusinessException>(() => _dailyLogAppService.SubmitAsync(created.Id));
        }
    }

    [Fact]
    public async Task ReturnToDraft_WhenRevisionRequested_ShouldReturnToDraft()
    {
        var (userId, mentorId, _) = await CreateActiveInternAsync("revision.returntodraft");
        var created = await CreateDraftLogAsync(userId);
        await AddItemAsync(userId, created.Id, "Bir madde", 30);
        await SubmitAsync(userId, created.Id);

        using (LoginAs(mentorId))
        {
            await _mentorReviewAppService.RequestRevisionAsync(created.Id, new RequestDailyLogRevisionInput
            {
                Comment = "Eksik madde var."
            });
        }

        DailyLogDetailDto result;
        using (LoginAs(userId))
        {
            result = await _dailyLogAppService.ReturnToDraftAsync(created.Id);
        }

        result.Status.ShouldBe(DailyLogStatus.Draft);
    }

    [Fact]
    public async Task ReturnToDraft_WhenNotOwner_ShouldFail()
    {
        var (userId, mentorId, _) = await CreateActiveInternAsync("revision.returntodraft.notowner");
        var created = await CreateDraftLogAsync(userId);
        await AddItemAsync(userId, created.Id, "Bir madde", 30);
        await SubmitAsync(userId, created.Id);

        using (LoginAs(mentorId))
        {
            await _mentorReviewAppService.RequestRevisionAsync(created.Id, new RequestDailyLogRevisionInput
            {
                Comment = "Eksik madde var."
            });

            await Assert.ThrowsAsync<BusinessException>(() => _dailyLogAppService.ReturnToDraftAsync(created.Id));
        }
    }

    private IDisposable LoginAs(Guid userId)
    {
        return _currentPrincipalAccessor.Change(new Claim(AbpClaimTypes.UserId, userId.ToString()));
    }

    private async Task<(Guid UserId, Guid MentorId, InternProfileDetailDto Profile)> CreateActiveInternAsync(
        string namePrefix,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var userId = await CreateUserAsync($"intern.{namePrefix}");
        var mentorId = await CreateUserAsync($"mentor.{namePrefix}");
        var workplaceId = await CreateWorkplaceAsync($"Şirket {namePrefix}");

        var profile = await _internProfileAppService.CreateAsync(
            BuildCreateProfileDto(userId, mentorId, workplaceId, startDate, endDate));

        await _internProfileAppService.StartAsync(profile.Id);

        return (userId, mentorId, profile);
    }

    private async Task<DailyLogDetailDto> CreateDraftLogAsync(Guid userId, DateTime? logDate = null)
    {
        using (LoginAs(userId))
        {
            return await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = logDate ?? DateTime.Today });
        }
    }

    private async Task<DailyLogDetailDto> AddItemAsync(Guid ownerUserId, Guid dailyLogId, string title, int minutes)
    {
        using (LoginAs(ownerUserId))
        {
            return await _dailyLogAppService.AddItemAsync(dailyLogId, new AddDailyLogItemInput
            {
                Title = title,
                WorkType = WorkType.Development,
                DurationMinutes = minutes,
                IsCompleted = true
            });
        }
    }

    private async Task<DailyLogDetailDto> SubmitAsync(Guid ownerUserId, Guid dailyLogId)
    {
        using (LoginAs(ownerUserId))
        {
            return await _dailyLogAppService.SubmitAsync(dailyLogId);
        }
    }

    private async Task ApproveAsMentorAsync(Guid mentorUserId, Guid dailyLogId)
    {
        using (LoginAs(mentorUserId))
        {
            await _mentorReviewAppService.ApproveAsync(dailyLogId, new ApproveDailyLogReviewInput());
        }
    }

    private static CreateInternProfileDto BuildCreateProfileDto(
        Guid userId,
        Guid mentorId,
        Guid workplaceId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        return new CreateInternProfileDto
        {
            UserId = userId,
            MentorUserId = mentorId,
            WorkplaceId = workplaceId,
            University = "Test Üniversitesi",
            SchoolDepartment = "Bilgisayar Mühendisliği",
            StudentNumber = "2023123456",
            InternshipStartDate = startDate ?? DateTime.Today.AddDays(-30),
            InternshipEndDate = endDate ?? DateTime.Today.AddDays(30),
            RequiredWorkDays = 20
        };
    }

    private async Task<Guid> CreateUserAsync(string userName)
    {
        var userId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var user = new IdentityUser(userId, userName, $"{userName}@example.com");
            var result = await _identityUserManager.CreateAsync(user);
            result.Succeeded.ShouldBeTrue(string.Join(", ", result.Errors.Select(x => x.Description)));
        });

        return userId;
    }

    private async Task<Guid> CreateWorkplaceAsync(string name)
    {
        var workplace = await _workplaceAppService.CreateAsync(new CreateWorkplaceDto
        {
            Name = name,
            DistrictId = InternshipJournalSeedIds.Districts.Kadikoy,
            AddressLine = "Test Mahallesi No: 1"
        });

        return workplace.Id;
    }
}
