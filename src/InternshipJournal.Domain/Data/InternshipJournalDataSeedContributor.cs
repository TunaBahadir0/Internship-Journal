using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InternshipJournal.Locations;
using InternshipJournal.Permissions;
using InternshipJournal.Skills;
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

    private readonly IRepository<Country, Guid> _countryRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IRepository<District, Guid> _districtRepository;
    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionManager _permissionManager;

    public InternshipJournalDataSeedContributor(
        IRepository<Country, Guid> countryRepository,
        IRepository<Province, Guid> provinceRepository,
        IRepository<District, Guid> districtRepository,
        IRepository<Skill, Guid> skillRepository,
        IdentityRoleManager roleManager,
        IPermissionManager permissionManager)
    {
        _countryRepository = countryRepository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _skillRepository = skillRepository;
        _roleManager = roleManager;
        _permissionManager = permissionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedCountriesAsync();
        await SeedProvincesAsync();
        await SeedDistrictsAsync();
        await SeedSkillsAsync();
        await SeedRolesAsync();
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
