using System;
using InternshipJournal.Consts;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.Locations;

public class District : FullAuditedAggregateRoot<Guid>
{
    public Guid ProvinceId { get; private set; }

    public string? Code { get; private set; }

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    protected District()
    {
        /* Required by EF Core. */
    }

    internal District(
        Guid id,
        Guid provinceId,
        [NotNull] string name,
        [CanBeNull] string code = null,
        bool isActive = true) : base(id)
    {
        ProvinceId = provinceId;
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.MaxNameLength);
        Code = Check.Length(code, nameof(code), DistrictConsts.MaxCodeLength);
        IsActive = isActive;
    }

    public void Rename([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.MaxNameLength);
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
