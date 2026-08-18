using Volo.Abp.Application.Dtos;

namespace InternshipJournal.Workplaces;

public class GetWorkplaceListInput : PagedResultRequestDto
{
    public string? Filter { get; set; }

    public bool? IsActive { get; set; }
}
