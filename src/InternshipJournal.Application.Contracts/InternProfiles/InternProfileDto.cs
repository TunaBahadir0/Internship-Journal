using System;
using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.InternProfiles;

public class InternProfileDto : EntityDto<Guid>
{
    public string UserName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string WorkplaceName { get; set; } = null!;

    public DateTime InternshipStartDate { get; set; }

    public DateTime InternshipEndDate { get; set; }

    public InternshipStatus Status { get; set; }
}
