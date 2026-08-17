using System;
using InternshipJournal.Consts;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.Locations;

public class Province : FullAuditedAggregateRoot<Guid>
{
    public Guid CountryId { get; private set; }

    public string? Code { get; private set; }

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    protected Province()
    {
        /* Required by EF Core. */
    }

    internal Province(
        Guid id,
        Guid countryId,
        [NotNull] string name,
        [CanBeNull] string code = null,
        bool isActive = true) : base(id)
    {
        CountryId = countryId;
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ProvinceConsts.MaxNameLength);
        Code = Check.Length(code, nameof(code), ProvinceConsts.MaxCodeLength);
        IsActive = isActive;
    }

    public void Rename([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ProvinceConsts.MaxNameLength);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
