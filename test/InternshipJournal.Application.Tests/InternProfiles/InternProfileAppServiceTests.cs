using System;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.Data;
using InternshipJournal.Workplaces;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Xunit;

namespace InternshipJournal.InternProfiles;

public abstract class InternProfileAppServiceTests<TStartupModule> : InternshipJournalApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInternProfileAppService _internProfileAppService;
    private readonly IWorkplaceAppService _workplaceAppService;
    private readonly IdentityUserManager _identityUserManager;

    protected InternProfileAppServiceTests()
    {
        _internProfileAppService = GetRequiredService<IInternProfileAppService>();
        _workplaceAppService = GetRequiredService<IWorkplaceAppService>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
    }

    [Fact]
    public async Task Create_WhenDateRangeInvalid_ShouldFail()
    {
        var userId = await CreateUserAsync("intern.datefail");
        var mentorId = await CreateUserAsync("mentor.datefail");
        var workplaceId = await CreateWorkplaceAsync("Tarih Testi Şirketi");

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _internProfileAppService.CreateAsync(BuildCreateDto(
                userId, mentorId, workplaceId,
                startDate: DateTime.Today,
                endDate: DateTime.Today.AddDays(-1)));
        });
    }

    [Fact]
    public async Task Create_WhenWorkplaceInactive_ShouldFail()
    {
        var userId = await CreateUserAsync("intern.workplaceinactive");
        var mentorId = await CreateUserAsync("mentor.workplaceinactive");
        var workplaceId = await CreateWorkplaceAsync("Pasif Çalışma Yeri Testi");

        await _workplaceAppService.DeactivateAsync(workplaceId);

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, workplaceId));
        });
    }

    [Fact]
    public async Task Create_WhenWorkplaceDoesNotExist_ShouldFail()
    {
        var userId = await CreateUserAsync("intern.noworkplace");
        var mentorId = await CreateUserAsync("mentor.noworkplace");

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, Guid.NewGuid()));
        });
    }

    [Fact]
    public async Task Create_WhenMentorDoesNotExist_ShouldFail()
    {
        var userId = await CreateUserAsync("intern.nomentor");
        var workplaceId = await CreateWorkplaceAsync("Mentor Testi Şirketi");

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _internProfileAppService.CreateAsync(BuildCreateDto(userId, Guid.NewGuid(), workplaceId));
        });
    }

    [Fact]
    public async Task Create_WhenUserHasActiveProfile_ShouldFail()
    {
        var userId = await CreateUserAsync("intern.doubleactive");
        var mentorId = await CreateUserAsync("mentor.doubleactive");
        var workplaceId = await CreateWorkplaceAsync("İlk Çalışma Yeri Testi");

        var first = await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, workplaceId));
        await _internProfileAppService.StartAsync(first.Id);

        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, workplaceId));
        });
    }

    [Fact]
    public async Task ChangeWorkplace_WhenWorkplaceValid_ShouldChange()
    {
        var userId = await CreateUserAsync("intern.changeworkplace");
        var mentorId = await CreateUserAsync("mentor.changeworkplace");
        var workplace1 = await CreateWorkplaceAsync("Eski Çalışma Yeri");
        var workplace2 = await CreateWorkplaceAsync("Yeni Çalışma Yeri");

        var created = await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, workplace1));

        var updated = await _internProfileAppService.UpdateAsync(created.Id, new UpdateInternProfileDto
        {
            MentorUserId = mentorId,
            WorkplaceId = workplace2,
            University = created.University,
            SchoolDepartment = created.SchoolDepartment,
            StudentNumber = created.StudentNumber,
            InternshipStartDate = created.InternshipStartDate,
            InternshipEndDate = created.InternshipEndDate,
            RequiredWorkDays = created.RequiredWorkDays
        });

        updated.WorkplaceId.ShouldBe(workplace2);
        updated.WorkplaceName.ShouldBe("Yeni Çalışma Yeri");
    }

    [Fact]
    public async Task Start_WhenProfileDraft_ShouldStart()
    {
        var userId = await CreateUserAsync("intern.start");
        var mentorId = await CreateUserAsync("mentor.start");
        var workplaceId = await CreateWorkplaceAsync("Başlatma Testi Şirketi");

        var created = await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, workplaceId));

        await _internProfileAppService.StartAsync(created.Id);

        var result = await _internProfileAppService.GetAsync(created.Id);
        result.Status.ShouldBe(Enums.InternshipStatus.Active);
    }

    [Fact]
    public async Task Complete_WhenProfileActive_ShouldComplete()
    {
        var userId = await CreateUserAsync("intern.complete");
        var mentorId = await CreateUserAsync("mentor.complete");
        var workplaceId = await CreateWorkplaceAsync("Tamamlama Testi Şirketi");

        var created = await _internProfileAppService.CreateAsync(BuildCreateDto(userId, mentorId, workplaceId));
        await _internProfileAppService.StartAsync(created.Id);

        await _internProfileAppService.CompleteAsync(created.Id);

        var result = await _internProfileAppService.GetAsync(created.Id);
        result.Status.ShouldBe(Enums.InternshipStatus.Completed);
    }

    private static CreateInternProfileDto BuildCreateDto(
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
            InternshipStartDate = startDate ?? DateTime.Today,
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
