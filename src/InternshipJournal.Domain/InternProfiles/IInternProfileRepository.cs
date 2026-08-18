using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InternshipJournal.Enums;
using Volo.Abp.Domain.Repositories;

namespace InternshipJournal.InternProfiles;

public interface IInternProfileRepository : IRepository<InternProfile, Guid>
{
    Task<InternProfile?> FindByUserIdAsync(Guid userId);

    Task<InternProfile?> GetActiveByUserIdAsync(Guid userId);

    Task<bool> HasActiveProfileAsync(Guid userId);

    Task<InternProfileWithWorkplace?> GetWithWorkplaceAsync(Guid id);

    Task<InternProfileWithDetails?> GetWithMentorAndWorkplaceAsync(Guid id);

    Task<List<InternProfileWithDetails>> GetListWithDetailsAsync(
        string? filter = null,
        InternshipStatus? status = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue);
}
