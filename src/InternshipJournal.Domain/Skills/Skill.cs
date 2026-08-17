using System;
using InternshipJournal.Consts;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.Skills;

public class Skill : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;

    public string? Category { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    protected Skill()
    {
        /* Required by EF Core. */
    }

    internal Skill(
        Guid id,
        [NotNull] string name,
        [CanBeNull] string category = null,
        [CanBeNull] string description = null,
        bool isActive = true) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), SkillConsts.MaxNameLength);
        Category = Check.Length(category, nameof(category), SkillConsts.MaxCategoryLength);
        Description = Check.Length(description, nameof(description), SkillConsts.MaxDescriptionLength);
        IsActive = isActive;
    }

    public void Rename([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), SkillConsts.MaxNameLength);
    }

    public void ChangeCategory([CanBeNull] string category)
    {
        Category = Check.Length(category, nameof(category), SkillConsts.MaxCategoryLength);
    }

    public void ChangeDescription([CanBeNull] string description)
    {
        Description = Check.Length(description, nameof(description), SkillConsts.MaxDescriptionLength);
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
