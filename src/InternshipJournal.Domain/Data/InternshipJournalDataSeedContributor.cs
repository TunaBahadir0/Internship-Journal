using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InternshipJournal.DailyLogs;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using InternshipJournal.Locations;
using InternshipJournal.MentorReviews;
using InternshipJournal.Permissions;
using InternshipJournal.Skills;
using InternshipJournal.Workplaces;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace InternshipJournal.Data;

public class InternshipJournalDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public const string InternRoleName = "Stajyer";
    public const string MentorRoleName = "Mentor";

    // Demo/deneme kullanıcıları için sabit şifre - admin ile aynı, kullanıcının zaten bildiği bir değer.
    private const string DemoUserPassword = "1q2w3E*";
    private const string DemoInternUserName = "stajyer1";
    private const string DemoMentorUserName = "mentor1";
    private const string DemoWorkplaceName = "Örnek Yazılım A.Ş.";

    private readonly IRepository<Country, Guid> _countryRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IRepository<District, Guid> _districtRepository;
    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionManager _permissionManager;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IWorkplaceRepository _workplaceRepository;
    private readonly WorkplaceManager _workplaceManager;
    private readonly IInternProfileRepository _internProfileRepository;
    private readonly InternProfileManager _internProfileManager;
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly DailyLogManager _dailyLogManager;
    private readonly IMentorReviewRepository _mentorReviewRepository;
    private readonly MentorReviewManager _mentorReviewManager;

    public InternshipJournalDataSeedContributor(
        IRepository<Country, Guid> countryRepository,
        IRepository<Province, Guid> provinceRepository,
        IRepository<District, Guid> districtRepository,
        IRepository<Skill, Guid> skillRepository,
        IdentityRoleManager roleManager,
        IPermissionManager permissionManager,
        IdentityUserManager identityUserManager,
        IWorkplaceRepository workplaceRepository,
        WorkplaceManager workplaceManager,
        IInternProfileRepository internProfileRepository,
        InternProfileManager internProfileManager,
        IDailyLogRepository dailyLogRepository,
        DailyLogManager dailyLogManager,
        IMentorReviewRepository mentorReviewRepository,
        MentorReviewManager mentorReviewManager)
    {
        _countryRepository = countryRepository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _skillRepository = skillRepository;
        _roleManager = roleManager;
        _permissionManager = permissionManager;
        _identityUserManager = identityUserManager;
        _workplaceRepository = workplaceRepository;
        _workplaceManager = workplaceManager;
        _internProfileRepository = internProfileRepository;
        _internProfileManager = internProfileManager;
        _dailyLogRepository = dailyLogRepository;
        _dailyLogManager = dailyLogManager;
        _mentorReviewRepository = mentorReviewRepository;
        _mentorReviewManager = mentorReviewManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedCountriesAsync();
        await SeedProvincesAsync();
        await SeedDistrictsAsync();
        await SeedSkillsAsync();
        await SeedRolesAsync();
        await SeedDemoDataAsync();
    }

    private async Task SeedRolesAsync()
    {
        await GetOrCreateRoleAsync(InternRoleName);
        await GrantPermissionsToRoleAsync(
            InternRoleName,
            InternshipJournalPermissions.DailyLogs.Default,
            InternshipJournalPermissions.DailyLogs.Create,
            InternshipJournalPermissions.DailyLogs.Edit,
            InternshipJournalPermissions.DailyLogs.Submit);

        await GetOrCreateRoleAsync(MentorRoleName);
        await GrantPermissionsToRoleAsync(
            MentorRoleName,
            InternshipJournalPermissions.DailyLogs.Default,
            InternshipJournalPermissions.Reviews.Default,
            InternshipJournalPermissions.Reviews.Approve,
            InternshipJournalPermissions.Reviews.RequestRevision);
    }

    // Uçtan uca deneme yapılabilmesi için: bir stajyer, bir mentor, bir çalışma yeri, aktif bir
    // staj profili ve taslak/gönderilmiş/onaylanmış birer örnek günlük. DbMigrator her çalıştığında
    // idempotent şekilde kontrol eder (zaten varsa tekrar oluşturmaz).
    private async Task SeedDemoDataAsync()
    {
        var internUser = await GetOrCreateDemoUserAsync(DemoInternUserName, "Ayşe", "Yılmaz", InternRoleName);
        var mentorUser = await GetOrCreateDemoUserAsync(DemoMentorUserName, "Mehmet", "Demir", MentorRoleName);

        var workplace = await GetOrCreateDemoWorkplaceAsync();

        var internProfile = await GetOrCreateDemoInternProfileAsync(internUser.Id, mentorUser.Id, workplace.Id);

        await SeedDraftDailyLogAsync(internProfile.Id, DateTime.Now.Date.AddDays(-2));
        await SeedSubmittedDailyLogAsync(internProfile.Id, DateTime.Now.Date.AddDays(-1));
        await SeedApprovedDailyLogAsync(internProfile.Id, mentorUser.Id, DateTime.Now.Date.AddDays(-7));
    }

    private async Task<IdentityUser> GetOrCreateDemoUserAsync(string userName, string name, string surname, string roleName)
    {
        var existing = await _identityUserManager.FindByNameAsync(userName);
        if (existing != null)
        {
            return existing;
        }

        var user = new IdentityUser(Guid.NewGuid(), userName, $"{userName}@internshipjournal.local")
        {
            Name = name,
            Surname = surname
        };

        var createResult = await _identityUserManager.CreateAsync(user, DemoUserPassword);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"'{userName}' kullanıcısı oluşturulamadı: {string.Join(", ", createResult.Errors.Select(x => x.Description))}");
        }

        var roleResult = await _identityUserManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"'{userName}' kullanıcısına '{roleName}' rolü verilemedi: {string.Join(", ", roleResult.Errors.Select(x => x.Description))}");
        }

        return user;
    }

    private async Task<Workplace> GetOrCreateDemoWorkplaceAsync()
    {
        var existing = await _workplaceRepository.FindByNameAsync(DemoWorkplaceName);
        if (existing != null)
        {
            return existing;
        }

        var workplace = await _workplaceManager.CreateAsync(
            DemoWorkplaceName,
            InternshipJournalSeedIds.Districts.Kadikoy,
            "Örnek Mahallesi, Test Caddesi No:1",
            postalCode: "34710",
            taxNumber: null,
            phone: "02121234567",
            email: "info@ornekyazilim.com",
            website: "https://www.ornekyazilim.com",
            latitude: null,
            longitude: null);

        await _workplaceRepository.InsertAsync(workplace, autoSave: true);
        return workplace;
    }

    private async Task<InternProfile> GetOrCreateDemoInternProfileAsync(Guid internUserId, Guid mentorUserId, Guid workplaceId)
    {
        var existing = await _internProfileRepository.FindByUserIdAsync(internUserId);
        if (existing != null)
        {
            return existing;
        }

        var period = new DateRange(DateTime.Now.Date.AddMonths(-3), DateTime.Now.Date.AddMonths(9));
        var profile = await _internProfileManager.CreateAsync(
            internUserId,
            mentorUserId,
            workplaceId,
            "Örnek Üniversitesi",
            "Bilgisayar Mühendisliği",
            "20210001",
            period,
            60);

        profile.Start();

        await _internProfileRepository.InsertAsync(profile, autoSave: true);
        return profile;
    }

    private async Task SeedDraftDailyLogAsync(Guid internProfileId, DateTime logDate)
    {
        if (await _dailyLogRepository.ExistsForDateAsync(internProfileId, logDate))
        {
            return;
        }

        var log = await _dailyLogManager.CreateAsync(internProfileId, logDate, "İlk staj günüm, ortam kurulumu ve tanışma.");
        log.AddItem("Geliştirme ortamı kurulumu", "Visual Studio ve .NET SDK kurulumu yapıldı.", WorkType.Setup, 120, true);
        log.AddItem("Ekip tanışma toplantısı", null, WorkType.Meeting, 60, true);
        await _dailyLogManager.AddSkillAsync(log, InternshipJournalSeedIds.Skills.Git, LearningLevel.Introduced, "Temel git komutlarını öğrendim.");

        await _dailyLogRepository.InsertAsync(log, autoSave: true);
    }

    private async Task SeedSubmittedDailyLogAsync(Guid internProfileId, DateTime logDate)
    {
        if (await _dailyLogRepository.ExistsForDateAsync(internProfileId, logDate))
        {
            return;
        }

        var log = await _dailyLogManager.CreateAsync(internProfileId, logDate, "API endpoint geliştirme.");
        log.AddItem("Kullanıcı listeleme endpoint'i", "GET /api/users uç noktası eklendi.", WorkType.Development, 180, true);
        log.AddItem("Unit testleri yazma", null, WorkType.Testing, 90, true);
        log.AddProblem(
            "Migration hatası",
            "EF Core migration eklerken FK hatası aldım.",
            errorMessage: "The ADD CONSTRAINT statement conflicted with the FOREIGN KEY constraint.",
            attemptedSolutions: "Migration'ı silip yeniden oluşturdum.",
            rootCause: "Yanlış tabloya FK eklemiştim.",
            finalSolution: "Doğru tabloya FK ekleyip migration'ı yeniden oluşturdum.",
            usedArtificialIntelligence: true,
            aiToolName: "Claude",
            aiPromptSummary: "EF Core FK constraint hatasının olası nedenlerini sordum.",
            aiSuggestion: "Migration dosyasındaki FK tanımını ve tablo adını kontrol etmemi önerdi.",
            aiSuggestionAccepted: true,
            aiRejectionReason: null);
        log.Submit();

        await _dailyLogRepository.InsertAsync(log, autoSave: true);
    }

    private async Task SeedApprovedDailyLogAsync(Guid internProfileId, Guid mentorUserId, DateTime logDate)
    {
        if (await _dailyLogRepository.ExistsForDateAsync(internProfileId, logDate))
        {
            return;
        }

        var log = await _dailyLogManager.CreateAsync(internProfileId, logDate, "Veritabanı şeması tasarımı.");
        log.AddItem("ER diyagramı çizimi", "Ana tablolar ve ilişkiler belirlendi.", WorkType.Research, 150, true);
        log.Submit();
        await _dailyLogRepository.InsertAsync(log, autoSave: true);

        var (review, approvedLog) = await _mentorReviewManager.ApproveAsync(log.Id, mentorUserId, "Güzel bir başlangıç, devam et.");
        await _mentorReviewRepository.InsertAsync(review, autoSave: true);
        await _dailyLogRepository.UpdateAsync(approvedLog, autoSave: true);
    }

    private async Task<IdentityRole> GetOrCreateRoleAsync(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);
        if (role != null)
        {
            return role;
        }

        role = new IdentityRole(Guid.NewGuid(), name)
        {
            IsPublic = true
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"'{name}' rolü oluşturulamadı: {string.Join(", ", result.Errors.Select(x => x.Description))}");
        }

        return role;
    }

    private const string RoleProviderName = "R";

    private async Task GrantPermissionsToRoleAsync(string roleName, params string[] permissionNames)
    {
        foreach (var permissionName in permissionNames)
        {
            await _permissionManager.SetAsync(permissionName, RoleProviderName, roleName, true);
        }
    }

    private async Task SeedCountriesAsync()
    {
        await InsertIfNotExistsAsync(_countryRepository, InternshipJournalSeedIds.Countries.Turkey,
            c => c.Code == "TR",
            id => new Country(id, "TR", "Türkiye"));

        await InsertIfNotExistsAsync(_countryRepository, InternshipJournalSeedIds.Countries.Germany,
            c => c.Code == "DE",
            id => new Country(id, "DE", "Almanya"));

        await InsertIfNotExistsAsync(_countryRepository, InternshipJournalSeedIds.Countries.Netherlands,
            c => c.Code == "NL",
            id => new Country(id, "NL", "Hollanda"));
    }

    private async Task SeedProvincesAsync()
    {
        var turkeyId = InternshipJournalSeedIds.Countries.Turkey;

        await InsertIfNotExistsAsync(_provinceRepository, InternshipJournalSeedIds.Provinces.Istanbul,
            p => p.CountryId == turkeyId && p.Name == "İstanbul",
            id => new Province(id, turkeyId, "İstanbul"));

        await InsertIfNotExistsAsync(_provinceRepository, InternshipJournalSeedIds.Provinces.Ankara,
            p => p.CountryId == turkeyId && p.Name == "Ankara",
            id => new Province(id, turkeyId, "Ankara"));

        await InsertIfNotExistsAsync(_provinceRepository, InternshipJournalSeedIds.Provinces.Izmir,
            p => p.CountryId == turkeyId && p.Name == "İzmir",
            id => new Province(id, turkeyId, "İzmir"));

        await InsertIfNotExistsAsync(_provinceRepository, InternshipJournalSeedIds.Provinces.Bursa,
            p => p.CountryId == turkeyId && p.Name == "Bursa",
            id => new Province(id, turkeyId, "Bursa"));

        await InsertIfNotExistsAsync(_provinceRepository, InternshipJournalSeedIds.Provinces.Kocaeli,
            p => p.CountryId == turkeyId && p.Name == "Kocaeli",
            id => new Province(id, turkeyId, "Kocaeli"));
    }

    private async Task SeedDistrictsAsync()
    {
        var istanbulId = InternshipJournalSeedIds.Provinces.Istanbul;
        var ankaraId = InternshipJournalSeedIds.Provinces.Ankara;
        var izmirId = InternshipJournalSeedIds.Provinces.Izmir;

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Kadikoy,
            d => d.ProvinceId == istanbulId && d.Name == "Kadıköy",
            id => new District(id, istanbulId, "Kadıköy"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Uskudar,
            d => d.ProvinceId == istanbulId && d.Name == "Üsküdar",
            id => new District(id, istanbulId, "Üsküdar"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Sisli,
            d => d.ProvinceId == istanbulId && d.Name == "Şişli",
            id => new District(id, istanbulId, "Şişli"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Besiktas,
            d => d.ProvinceId == istanbulId && d.Name == "Beşiktaş",
            id => new District(id, istanbulId, "Beşiktaş"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Atasehir,
            d => d.ProvinceId == istanbulId && d.Name == "Ataşehir",
            id => new District(id, istanbulId, "Ataşehir"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Cankaya,
            d => d.ProvinceId == ankaraId && d.Name == "Çankaya",
            id => new District(id, ankaraId, "Çankaya"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Yenimahalle,
            d => d.ProvinceId == ankaraId && d.Name == "Yenimahalle",
            id => new District(id, ankaraId, "Yenimahalle"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Kecioren,
            d => d.ProvinceId == ankaraId && d.Name == "Keçiören",
            id => new District(id, ankaraId, "Keçiören"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Konak,
            d => d.ProvinceId == izmirId && d.Name == "Konak",
            id => new District(id, izmirId, "Konak"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Bornova,
            d => d.ProvinceId == izmirId && d.Name == "Bornova",
            id => new District(id, izmirId, "Bornova"));

        await InsertIfNotExistsAsync(_districtRepository, InternshipJournalSeedIds.Districts.Karsiyaka,
            d => d.ProvinceId == izmirId && d.Name == "Karşıyaka",
            id => new District(id, izmirId, "Karşıyaka"));
    }

    private async Task SeedSkillsAsync()
    {
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.CSharp, "C#");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.DotNet, ".NET");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.AbpFramework, "ABP Framework");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.EntityFrameworkCore, "Entity Framework Core");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.PostgreSql, "PostgreSQL");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.Docker, "Docker");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.Git, "Git");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.Ddd, "DDD");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.RazorPages, "Razor Pages");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.HtmlCss, "HTML/CSS");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.JavaScript, "JavaScript");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.UnitTesting, "Unit Testing");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.ProblemSolving, "Problem Solving");
        await InsertSkillIfNotExistsAsync(InternshipJournalSeedIds.Skills.AiAssistedCoding, "Yapay Zekâ ile Kodlama");
    }

    private Task InsertSkillIfNotExistsAsync(Guid id, string name)
    {
        return InsertIfNotExistsAsync(_skillRepository, id,
            s => s.Name == name,
            skillId => new Skill(skillId, name));
    }

    private static async Task InsertIfNotExistsAsync<TEntity>(
        IRepository<TEntity, Guid> repository,
        Guid id,
        Expression<Func<TEntity, bool>> alreadyExistsPredicate,
        Func<Guid, TEntity> factory)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<Guid>
    {
        var existing = await repository.GetListAsync(alreadyExistsPredicate);
        if (existing.Any())
        {
            return;
        }

        await repository.InsertAsync(factory(id));
    }
}
