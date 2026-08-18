using System;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.Workplaces;

public class WorkplaceDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public string DistrictName { get; set; } = null!;

    public string ProvinceName { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public bool IsActive { get; set; }
}
