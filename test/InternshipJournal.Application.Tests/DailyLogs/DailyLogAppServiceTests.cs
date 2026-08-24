using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InternshipJournal.Data;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
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
    private readonly IdentityUserManager _identityUserManager;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    protected DailyLogAppServiceTests()
    {
        _dailyLogAppService = GetRequiredService<IDailyLogAppService>();
        _internProfileAppService = GetRequiredService<IInternProfileAppService>();
        _workplaceAppService = GetRequiredService<IWorkplaceAppService>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
    }

    [Fact]
    public async Task Create_WithValidDate_ShouldCreate()
    {
        var (userId, _) = await CreateActiveInternAsync("create.valid");

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
        var (userId, _) = await CreateActiveInternAsync("create.duplicate");

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
        var (userId1, _) = await CreateActiveInternAsync("list.intern1");
        var (userId2, _) = await CreateActiveInternAsync("list.intern2");

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
        var (userId, _) = await CreateActiveInternAsync("list.datefilter", startDate: DateTime.Today.AddDays(-10));

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
    public async Task UpdateSummary_WhenLogEditable_ShouldUpdate()
    {
        var (userId, _) = await CreateActiveInternAsync("update.editable");

        DailyLogDetailDto created;
        using (LoginAs(userId))
        {
            created = await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
        }

        var updated = await _dailyLogAppService.UpdateSummaryAsync(created.Id, new UpdateDailyLogSummaryDto
        {
            Summary = "Güncellenmiş özet"
        });

        updated.Summary.ShouldBe("Güncellenmiş özet");
    }

    [Fact]
    public async Task UpdateSummary_WhenLogApproved_ShouldFail()
    {
        var (userId, _) = await CreateActiveInternAsync("update.approved");

        DailyLogDetailDto created;
        using (LoginAs(userId))
        {
            created = await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
        }

        await SubmitAndApproveAsync(created.Id);

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _dailyLogAppService.UpdateSummaryAsync(created.Id, new UpdateDailyLogSummaryDto { Summary = "Değişmemeli" });
        });
    }

    [Fact]
    public async Task GetDetail_ShouldReturnChildCollections()
    {
        var (userId, _) = await CreateActiveInternAsync("detail.children");

        DailyLogDetailDto created;
        using (LoginAs(userId))
        {
            created = await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
        }

        await AddItemAsync(created.Id, "İlk madde", 60);

        var detail = await _dailyLogAppService.GetAsync(created.Id);

        detail.Items.Count.ShouldBe(1);
        detail.TotalMinutes.ShouldBe(60);
    }

    private IDisposable LoginAs(Guid userId)
    {
        return _currentPrincipalAccessor.Change(new Claim(AbpClaimTypes.UserId, userId.ToString()));
    }

    private async Task<(Guid UserId, InternProfileDetailDto Profile)> CreateActiveInternAsync(
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

        return (userId, profile);
    }

    private async Task AddItemAsync(Guid dailyLogId, string title, int minutes)
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var dailyLogRepository = GetRequiredService<IDailyLogRepository>();
            var dailyLog = await dailyLogRepository.GetWithDetailsAsync(dailyLogId);
            dailyLog!.AddItem(title, null, WorkType.Development, minutes, true);
            await dailyLogRepository.UpdateAsync(dailyLog);
        });
    }

    private async Task SubmitAndApproveAsync(Guid dailyLogId)
    {
        await AddItemAsync(dailyLogId, "Onay öncesi madde", 30);

        await WithUnitOfWorkAsync(async () =>
        {
            var dailyLogRepository = GetRequiredService<IDailyLogRepository>();
            var dailyLog = await dailyLogRepository.GetWithDetailsAsync(dailyLogId);
            dailyLog!.Submit();
            dailyLog.Approve();
            await dailyLogRepository.UpdateAsync(dailyLog);
        });
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
