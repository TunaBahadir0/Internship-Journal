using InternshipJournal.Enums;
using Volo.Abp.Application.Dtos;

namespace InternshipJournal.InternProfiles;

public class GetInternProfileListInput : PagedResultRequestDto
{
    public string? Filter { get; set; }

    public InternshipStatus? Status { get; set; }
}
