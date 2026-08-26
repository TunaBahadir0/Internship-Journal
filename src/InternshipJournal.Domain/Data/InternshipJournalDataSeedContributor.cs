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

    // Uçtan uca deneme/test yapılabilmesi için üç senaryo seed ediliyor:
    //   1) Aktif bir stajyerin taslak + gönderilmiş + onaylanmış günlükleri (stajyer1/mentor1).
    //   2) Düzeltme istenip düzeltildikten sonra tekrar gönderilen bir günlük - RevisionRequested
    //      akışını ve inceleme geçmişini göstermek için (stajyer2/mentor2, ayrı bir işyerinde).
    //   3) mentor1'e bağlı, stajı tamamlanmış (Completed) ikinci bir stajyer (stajyer3) - bir
    //      mentörün birden fazla stajyeri olabildiğini ve tamamlanmış bir profilin durumunu gösteriyor.
    //
    // Günlük tarihleri DateTime.Now'dan DEĞİL, ilgili profilin kendi InternshipPeriod.StartDate'inden
    // sabit gün farkıyla türetiliyor. Önceki sürüm DateTime.Now.Date.AddDays(-N) kullanıyordu; bu,
    // DbMigrator farklı bir takvim gününde tekrar çalıştığında ExistsForDateAsync kontrolünün hep
    // "yok" dönmesine ve her çalıştırmada aynı üç günlüğün tekrar tekrar eklenmesine yol açan gerçek
    // bir bug'dı (idempotent olması gerekirken değildi). Profilin StartDate'i bir kez oluşturulduktan
    // sonra veritabanında sabit kaldığı için, ona göre türetilen tarihler de her çalıştırmada aynı
    // kalıyor ve ExistsForDateAsync artık gerçekten "zaten var" diyebiliyor.
    private async Task SeedDemoDataAsync()
    {
        var mentor1 = await GetOrCreateDemoUserAsync(DemoMentorUserName, "Mehmet", "Demir", MentorRoleName);
        var mentor2 = await GetOrCreateDemoUserAsync("mentor2", "Zeynep", "Arslan", MentorRoleName);

        var workplace1 = await GetOrCreateDemoWorkplaceAsync(
            DemoWorkplaceName,
            InternshipJournalSeedIds.Districts.Kadikoy,
            "Örnek Mahallesi, Test Caddesi No:1",
            "34710",
            "02121234567",
            "info@ornekyazilim.com",
            "https://www.ornekyazilim.com");

        var workplace2 = await GetOrCreateDemoWorkplaceAsync(
            "Beta Teknoloji Ltd. Şti.",
            InternshipJournalSeedIds.Districts.Uskudar,
            "Beta Mahallesi, İnovasyon Caddesi No:7",
            "34664",
            "02163334455",
            "info@betateknoloji.com",
            "https://www.betateknoloji.com");

        var intern1 = await GetOrCreateDemoUserAsync(DemoInternUserName, "Ayşe", "Yılmaz", InternRoleName);
        var profile1 = await GetOrCreateDemoInternProfileAsync(
            intern1.Id, mentor1.Id, workplace1.Id,
            "Örnek Üniversitesi", "Bilgisayar Mühendisliği", "20210001", requiredWorkDays: 60);
        await SeedDraftSubmittedApprovedLogsAsync(profile1, mentor1.Id);

        var intern2 = await GetOrCreateDemoUserAsync("stajyer2", "Elif", "Kaya", InternRoleName);
        var profile2 = await GetOrCreateDemoInternProfileAsync(
            intern2.Id, mentor2.Id, workplace2.Id,
            "Örnek Üniversitesi", "Yazılım Mühendisliği", "20210002", requiredWorkDays: 40);
        await SeedRevisionRequestedThenResubmittedLogAsync(profile2, mentor2.Id);

        var intern3 = await GetOrCreateDemoUserAsync("stajyer3", "Can", "Öztürk", InternRoleName);
        var profile3 = await GetOrCreateDemoInternProfileAsync(
            intern3.Id, mentor1.Id, workplace1.Id,
            "Örnek Üniversitesi", "Bilgisayar Mühendisliği", "20200099", requiredWorkDays: 60);
        await SeedCompletedInternshipAsync(profile3, mentor1.Id);
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

    private async Task<Workplace> GetOrCreateDemoWorkplaceAsync(
        string name,
        Guid districtId,
        string addressLine,
        string postalCode,
        string phone,
        string email,
        string website)
    {
        var existing = await _workplaceRepository.FindByNameAsync(name);
        if (existing != null)
        {
            return existing;
        }

        var workplace = await _workplaceManager.CreateAsync(
            name,
            districtId,
            addressLine,
            postalCode: postalCode,
            taxNumber: null,
            phone: phone,
            email: email,
            website: website,
            latitude: null,
            longitude: null);

        await _workplaceRepository.InsertAsync(workplace, autoSave: true);
        return workplace;
    }

    private async Task<InternProfile> GetOrCreateDemoInternProfileAsync(
        Guid internUserId,
        Guid mentorUserId,
        Guid workplaceId,
        string university,
        string schoolDepartment,
        string studentNumber,
        int requiredWorkDays)
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
            university,
            schoolDepartment,
            studentNumber,
            period,
            requiredWorkDays);

        profile.Start();

        await _internProfileRepository.InsertAsync(profile, autoSave: true);
        return profile;
    }

    private async Task SeedDraftSubmittedApprovedLogsAsync(InternProfile internProfile, Guid mentorUserId)
    {
        // Profil daha önce (ör. admin ekranından elle) Completed/Cancelled durumuna alınmış olabilir -
        // DailyLogManager sadece Active bir profil için günlük oluşturulmasına izin veriyor, bu yüzden
        // burada da önce durumu kontrol ediyoruz; aksi halde profil zaten Active değilken günlük
        // eklemeye çalışmak BusinessException fırlatıp DbMigrator'ı tamamen durdururdu.
        if (internProfile.Status != InternshipStatus.Active)
        {
            return;
        }

        var periodStart = internProfile.InternshipPeriod.StartDate;

        await SeedDraftDailyLogAsync(internProfile.Id, periodStart.AddDays(10));
        await SeedSubmittedDailyLogAsync(internProfile.Id, periodStart.AddDays(11));
        await SeedApprovedDailyLogAsync(internProfile.Id, mentorUserId, periodStart.AddDays(3));
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

    // Düzeltme isteniyor, stajyer düzeltip tekrar gönderiyor: günlük yeniden Submitted durumunda
    // kalıyor ama inceleme geçmişinde bir RevisionRequested kaydı görünüyor - mentörün "tekrar
    // incele" akışını ve stajyerin düzeltme geçmişini görebildiği ekranı test etmek için.
    private async Task SeedRevisionRequestedThenResubmittedLogAsync(InternProfile internProfile, Guid mentorUserId)
    {
        if (internProfile.Status != InternshipStatus.Active)
        {
            return;
        }

        var logDate = internProfile.InternshipPeriod.StartDate.AddDays(5);

        if (await _dailyLogRepository.ExistsForDateAsync(internProfile.Id, logDate))
        {
            return;
        }

        var log = await _dailyLogManager.CreateAsync(internProfile.Id, logDate, "Kullanıcı arayüzü ekranları.");
        log.AddItem("Giriş ekranı tasarımı", "Statik HTML/CSS ile giriş ekranı hazırlandı.", WorkType.Development, 150, true);
        await _dailyLogManager.AddSkillAsync(log, InternshipJournalSeedIds.Skills.HtmlCss, LearningLevel.Practiced, "Flexbox ile düzen kurmayı öğrendim.");
        log.Submit();
        await _dailyLogRepository.InsertAsync(log, autoSave: true);

        var (revisionReview, revisedLog) = await _mentorReviewManager.RequestRevisionAsync(
            log.Id, mentorUserId, "Giriş ekranında form doğrulama mesajları eksik, lütfen ekleyip tekrar gönder.");
        await _mentorReviewRepository.InsertAsync(revisionReview, autoSave: true);
        await _dailyLogRepository.UpdateAsync(revisedLog, autoSave: true);

        revisedLog.ReturnToDraft();
        revisedLog.AddItem("Form doğrulama mesajları", "Boş alan ve geçersiz e-posta uyarıları eklendi.", WorkType.Development, 60, true);
        revisedLog.Submit();
        await _dailyLogRepository.UpdateAsync(revisedLog, autoSave: true);
    }

    // Stajı biten bir stajyer: bir günlüğü onaylanmış, profili Completed durumunda.
    private async Task SeedCompletedInternshipAsync(InternProfile internProfile, Guid mentorUserId)
    {
        var logDate = internProfile.InternshipPeriod.StartDate.AddDays(2);

        if (internProfile.Status == InternshipStatus.Active
            && !await _dailyLogRepository.ExistsForDateAsync(internProfile.Id, logDate))
        {
            var log = await _dailyLogManager.CreateAsync(internProfile.Id, logDate, "Kapanış raporu ve devir teslim.");
            log.AddItem("Devir teslim dokümantasyonu", "Kalan işler ve notlar sonraki stajyere aktarıldı.", WorkType.Documentation, 90, true);
            log.Submit();
            await _dailyLogRepository.InsertAsync(log, autoSave: true);

            var (review, approvedLog) = await _mentorReviewManager.ApproveAsync(
                log.Id, mentorUserId, "Staj boyunca gösterdiğin gelişim için teşekkürler.");
            await _mentorReviewRepository.InsertAsync(review, autoSave: true);
            await _dailyLogRepository.UpdateAsync(approvedLog, autoSave: true);
        }

        // Profil zaten Completed ise (önceki bir çalıştırmada tamamlanmışsa) Complete()'i tekrar
        // çağırıp BusinessException almamak için durumu kontrol ediyoruz.
        if (internProfile.Status == InternshipStatus.Active)
        {
            internProfile.Complete();
            await _internProfileRepository.UpdateAsync(internProfile, autoSave: true);
        }
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
