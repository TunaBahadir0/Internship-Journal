using System;
using InternshipJournal.Consts;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.Locations;

public class Country : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    protected Country()
    {
        /* Required by EF Core. */
    }

    internal Country(
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        bool isActive = true) : base(id)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), CountryConsts.MaxCodeLength);
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), CountryConsts.MaxNameLength);
        IsActive = isActive;
    }

    public void Rename([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), CountryConsts.MaxNameLength);
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
