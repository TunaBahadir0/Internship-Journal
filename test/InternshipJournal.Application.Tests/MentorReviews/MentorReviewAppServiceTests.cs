using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using InternshipJournal.Data;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using InternshipJournal.Workplaces;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Validation;
using Xunit;

namespace InternshipJournal.MentorReviews;

public abstract class MentorReviewAppServiceTests<TStartupModule> : InternshipJournalApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IMentorReviewAppService _mentorReviewAppService;
    private readonly IDailyLogAppService _dailyLogAppService;
    private readonly IInternProfileAppService _internProfileAppService;
    private readonly IWorkplaceAppService _workplaceAppService;
    private readonly IdentityUserManager _identityUserManager;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    protected MentorReviewAppServiceTests()
    {
        _mentorReviewAppService = GetRequiredService<IMentorReviewAppService>();
        _dailyLogAppService = GetRequiredService<IDailyLogAppService>();
        _internProfileAppService = GetRequiredService<IInternProfileAppService>();
        _workplaceAppService = GetRequiredService<IWorkplaceAppService>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
    }

    [Fact]
    public async Task ApproveAsync_WhenMentorMatches_ShouldApproveDailyLogAndCreateReview()
    {
        var (internId, mentorId, dailyLogId) = await CreateSubmittedDailyLogAsync("approve.match");

        MentorReviewDto review;
        using (LoginAs(mentorId))
        {
            review = await _mentorReviewAppService.ApproveAsync(dailyLogId, new ApproveDailyLogReviewInput
            {
                Comment = "Güzel iş çıkarmışsın."
            });
        }

        review.Decision.ShouldBe(MentorReviewDecision.Approved);

        DailyLogDetailDto dailyLog;
        using (LoginAs(mentorId))
        {
            dailyLog = await _dailyLogAppService.GetAsync(dailyLogId);
        }

        dailyLog.Status.ShouldBe(DailyLogStatus.Approved);
    }

    [Fact]
    public async Task ApproveAsync_WhenMentorDoesNotMatch_ShouldFail()
    {
        var (_, _, dailyLogId) = await CreateSubmittedDailyLogAsync("approve.mismatch");
        var otherMentorId = await CreateUserAsync("mentor.approve.mismatch.other");

        using (LoginAs(otherMentorId))
        {
            await Assert.ThrowsAsync<BusinessException>(() =>
                _mentorReviewAppService.ApproveAsync(dailyLogId, new ApproveDailyLogReviewInput()));
        }
    }

    [Fact]
    public async Task RequestRevisionAsync_WhenMentorMatches_ShouldChangeDailyLogStatusAndCreateReview()
    {
        var (internId, mentorId, dailyLogId) = await CreateSubmittedDailyLogAsync("revision.match");

        MentorReviewDto review;
        using (LoginAs(mentorId))
        {
            review = await _mentorReviewAppService.RequestRevisionAsync(dailyLogId, new RequestDailyLogRevisionInput
            {
                Comment = "Eksik madde var, tamamlar mısın?"
            });
        }

        review.Decision.ShouldBe(MentorReviewDecision.RevisionRequested);

        DailyLogDetailDto dailyLog;
        using (LoginAs(mentorId))
        {
            dailyLog = await _dailyLogAppService.GetAsync(dailyLogId);
        }

        dailyLog.Status.ShouldBe(DailyLogStatus.RevisionRequested);
    }

    [Fact]
    public async Task RequestRevisionAsync_WhenCommentWhitespace_ShouldFail()
    {
        // ABP'nin kendi [Required] doğrulaması boşluktan oluşan bir string'i de geçersiz sayıp
        // AppService metoduna hiç girmeden AbpValidationException fırlatıyor. Domain katmanındaki
        // (MentorReview.RequestRevision) boşluk kontrolü, Manager'ın doğrudan (validasyonsuz)
        // çağrıldığı senaryolar için ikinci bir savunma hattı — bkz. MentorReviewTests/
        // MentorReviewManagerTests, o katmanda BusinessException olarak doğrulanıyor.
        var (_, mentorId, dailyLogId) = await CreateSubmittedDailyLogAsync("revision.whitespace");

        using (LoginAs(mentorId))
        {
            await Assert.ThrowsAsync<AbpValidationException>(() =>
                _mentorReviewAppService.RequestRevisionAsync(dailyLogId, new RequestDailyLogRevisionInput
                {
                    Comment = "   "
                }));
        }
    }

    [Fact]
    public async Task GetListByDailyLogAsync_ShouldReturnReviewHistory()
    {
        var (_, mentorId, dailyLogId) = await CreateSubmittedDailyLogAsync("history");

        using (LoginAs(mentorId))
        {
            await _mentorReviewAppService.RequestRevisionAsync(dailyLogId, new RequestDailyLogRevisionInput
            {
                Comment = "Önce şunu düzelt."
            });
        }

        var reviews = await _mentorReviewAppService.GetListByDailyLogAsync(dailyLogId);

        reviews.Count.ShouldBe(1);
        reviews.Single().Decision.ShouldBe(MentorReviewDecision.RevisionRequested);
    }

    private IDisposable LoginAs(Guid userId)
    {
        return _currentPrincipalAccessor.Change(new Claim(AbpClaimTypes.UserId, userId.ToString()));
    }

    private async Task<(Guid InternId, Guid MentorId, Guid DailyLogId)> CreateSubmittedDailyLogAsync(string namePrefix)
    {
        var internId = await CreateUserAsync($"intern.{namePrefix}");
        var mentorId = await CreateUserAsync($"mentor.{namePrefix}");
        var workplaceId = await CreateWorkplaceAsync($"Şirket {namePrefix}");

        var profile = await _internProfileAppService.CreateAsync(new CreateInternProfileDto
        {
            UserId = internId,
            MentorUserId = mentorId,
            WorkplaceId = workplaceId,
            University = "Test Üniversitesi",
            SchoolDepartment = "Bilgisayar Mühendisliği",
            StudentNumber = "2023123456",
            InternshipStartDate = DateTime.Today.AddDays(-30),
            InternshipEndDate = DateTime.Today.AddDays(30),
            RequiredWorkDays = 20
        });
        await _internProfileAppService.StartAsync(profile.Id);

        DailyLogDetailDto dailyLog;
        using (LoginAs(internId))
        {
            dailyLog = await _dailyLogAppService.CreateAsync(new CreateDailyLogDto { LogDate = DateTime.Today });
            await _dailyLogAppService.AddItemAsync(dailyLog.Id, new AddDailyLogItemInput
            {
                Title = "Bir çalışma maddesi",
                WorkType = WorkType.Development,
                DurationMinutes = 60,
                IsCompleted = true
            });
            await _dailyLogAppService.SubmitAsync(dailyLog.Id);
        }

        return (internId, mentorId, dailyLog.Id);
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
