using Microsoft.EntityFrameworkCore;
using InternshipJournal.Consts;
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
    }
}
