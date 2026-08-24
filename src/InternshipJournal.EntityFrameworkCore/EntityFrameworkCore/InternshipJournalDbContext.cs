using Microsoft.EntityFrameworkCore;
using InternshipJournal.Consts;
using InternshipJournal.DailyLogs;
using InternshipJournal.InternProfiles;
using InternshipJournal.Locations;
using InternshipJournal.Skills;
using InternshipJournal.Workplaces;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace InternshipJournal.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class InternshipJournalDbContext :
    AbpDbContext<InternshipJournalDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Workplace> Workplaces { get; set; }
    public DbSet<InternProfile> InternProfiles { get; set; }
    public DbSet<DailyLog> DailyLogs { get; set; }
    public DbSet<DailyLogItem> DailyLogItems { get; set; }
    public DbSet<DailyLogSkill> DailyLogSkills { get; set; }
    public DbSet<ProblemSolvingEntry> ProblemSolvingEntries { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public InternshipJournalDbContext(DbContextOptions<InternshipJournalDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        builder.Entity<Country>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "Countries", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(CountryConsts.MaxCodeLength);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(CountryConsts.MaxNameLength);

            b.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<Province>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "Provinces", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code)
                .HasMaxLength(ProvinceConsts.MaxCodeLength);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ProvinceConsts.MaxNameLength);

            b.HasIndex(x => new { x.CountryId, x.Name }).IsUnique();

            b.HasOne<Country>()
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<District>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "Districts", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Code)
                .HasMaxLength(DistrictConsts.MaxCodeLength);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(DistrictConsts.MaxNameLength);

            b.HasIndex(x => new { x.ProvinceId, x.Name }).IsUnique();

            b.HasOne<Province>()
                .WithMany()
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Skill>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "Skills", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(SkillConsts.MaxNameLength);

            b.Property(x => x.Category)
                .HasMaxLength(SkillConsts.MaxCategoryLength);

            b.Property(x => x.Description)
                .HasMaxLength(SkillConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<Workplace>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "Workplaces", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(WorkplaceConsts.MaxNameLength);

            b.Property(x => x.TaxNumber)
                .HasMaxLength(WorkplaceConsts.MaxTaxNumberLength);

            b.Property(x => x.Phone)
                .HasMaxLength(WorkplaceConsts.MaxPhoneLength);

            b.Property(x => x.Email)
                .HasMaxLength(WorkplaceConsts.MaxEmailLength);

            b.Property(x => x.Website)
                .HasMaxLength(WorkplaceConsts.MaxWebsiteLength);

            b.Property(x => x.AddressLine)
                .IsRequired()
                .HasMaxLength(WorkplaceConsts.MaxAddressLineLength);

            b.Property(x => x.PostalCode)
                .HasMaxLength(WorkplaceConsts.MaxPostalCodeLength);

            b.Property(x => x.Latitude)
                .HasColumnType("decimal(9,6)");

            b.Property(x => x.Longitude)
                .HasColumnType("decimal(9,6)");

            b.HasIndex(x => x.Name).IsUnique();
            b.HasIndex(x => x.DistrictId);

            b.HasOne<District>()
                .WithMany()
                .HasForeignKey(x => x.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<InternProfile>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "InternProfiles", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.University)
                .IsRequired()
                .HasMaxLength(InternProfileConsts.MaxUniversityLength);

            b.Property(x => x.SchoolDepartment)
                .IsRequired()
                .HasMaxLength(InternProfileConsts.MaxSchoolDepartmentLength);

            b.Property(x => x.StudentNumber)
                .IsRequired()
                .HasMaxLength(InternProfileConsts.MaxStudentNumberLength);

            b.OwnsOne(x => x.InternshipPeriod, o =>
            {
                o.Property(p => p.StartDate)
                    .HasColumnName(nameof(InternProfile.InternshipPeriod) + "_" + nameof(DateRange.StartDate))
                    .IsRequired();

                o.Property(p => p.EndDate)
                    .HasColumnName(nameof(InternProfile.InternshipPeriod) + "_" + nameof(DateRange.EndDate))
                    .IsRequired();
            });

            b.Navigation(x => x.InternshipPeriod).IsRequired();

            b.HasIndex(x => x.UserId)
                .IsUnique()
                .HasFilter("\"Status\" = 1");

            b.HasOne<Workplace>()
                .WithMany()
                .HasForeignKey(x => x.WorkplaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<DailyLog>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "DailyLogs", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.LogDate)
                .IsRequired()
                .HasColumnType("date");

            b.Property(x => x.Summary)
                .HasMaxLength(DailyLogConsts.MaxSummaryLength);

            b.HasIndex(x => new { x.InternProfileId, x.LogDate }).IsUnique();
            b.HasIndex(x => x.Status);

            b.HasOne<InternProfile>()
                .WithMany()
                .HasForeignKey(x => x.InternProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey("DailyLogId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(x => x.Skills)
                .WithOne()
                .HasForeignKey("DailyLogId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(x => x.Skills).UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(x => x.Problems)
                .WithOne()
                .HasForeignKey("DailyLogId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(x => x.Problems).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<DailyLogItem>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "DailyLogItems", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(DailyLogItemConsts.MaxTitleLength);

            b.Property(x => x.Description)
                .HasMaxLength(DailyLogItemConsts.MaxDescriptionLength);
        });

        builder.Entity<DailyLogSkill>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "DailyLogSkills", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Note)
                .HasMaxLength(DailyLogSkillConsts.MaxNoteLength);

            b.HasIndex("DailyLogId", nameof(DailyLogSkill.SkillId)).IsUnique();

            b.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProblemSolvingEntry>(b =>
        {
            b.ToTable(InternshipJournalConsts.DbTablePrefix + "ProblemSolvingEntries", InternshipJournalConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(ProblemSolvingEntryConsts.MaxTitleLength);

            b.Property(x => x.ProblemDescription)
                .IsRequired()
                .HasMaxLength(ProblemSolvingEntryConsts.MaxProblemDescriptionLength);

            b.Property(x => x.ErrorMessage)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxErrorMessageLength);

            b.Property(x => x.AttemptedSolutions)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxAttemptedSolutionsLength);

            b.Property(x => x.RootCause)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxRootCauseLength);

            b.Property(x => x.FinalSolution)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxFinalSolutionLength);

            b.Property(x => x.AiToolName)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxAiToolNameLength);

            b.Property(x => x.AiPromptSummary)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxAiPromptSummaryLength);

            b.Property(x => x.AiSuggestion)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxAiSuggestionLength);

            b.Property(x => x.AiRejectionReason)
                .HasMaxLength(ProblemSolvingEntryConsts.MaxAiRejectionReasonLength);
        });
    }
}
