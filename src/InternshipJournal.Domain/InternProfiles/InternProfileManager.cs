using System;
using System.Threading.Tasks;
using InternshipJournal.Workplaces;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace InternshipJournal.InternProfiles;

public class InternProfileManager : DomainService
{
    private readonly IInternProfileRepository _internProfileRepository;
    private readonly IRepository<Workplace, Guid> _workplaceRepository;
    private readonly IdentityUserManager _identityUserManager;

    public InternProfileManager(
        IInternProfileRepository internProfileRepository,
        IRepository<Workplace, Guid> workplaceRepository,
        IdentityUserManager identityUserManager)
    {
        _internProfileRepository = internProfileRepository;
        _workplaceRepository = workplaceRepository;
        _identityUserManager = identityUserManager;
    }

    public async Task<InternProfile> CreateAsync(
        Guid userId,
        Guid mentorUserId,
        Guid workplaceId,
        string university,
        string schoolDepartment,
        string studentNumber,
        DateRange internshipPeriod,
        int requiredWorkDays)
    {
        await ValidateWorkplaceAsync(workplaceId);
        await ValidateMentorAsync(mentorUserId);
        await ValidateNoActiveProfileAsync(userId);

        return new InternProfile(
            GuidGenerator.Create(),
            userId,
            mentorUserId,
            workplaceId,
            university,
            schoolDepartment,
            studentNumber,
            internshipPeriod,
            requiredWorkDays);
    }

    public async Task ChangeWorkplaceAsync(InternProfile profile, Guid workplaceId)
    {
        await ValidateWorkplaceAsync(workplaceId);
        profile.ChangeWorkplace(workplaceId);
    }

    public async Task ChangeMentorAsync(InternProfile profile, Guid mentorUserId)
    {
        await ValidateMentorAsync(mentorUserId);
        profile.ChangeMentor(mentorUserId);
    }

    private async Task ValidateWorkplaceAsync(Guid workplaceId)
    {
        var workplace = await _workplaceRepository.FindAsync(workplaceId);
        if (workplace == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InternProfileWorkplaceNotFound);
        }

        if (!workplace.IsActive)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InternProfileWorkplaceInactive);
        }
    }

    private async Task ValidateMentorAsync(Guid mentorUserId)
    {
        var mentor = await _identityUserManager.FindByIdAsync(mentorUserId.ToString());
        if (mentor == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InternProfileMentorNotFound);
        }
    }

    private async Task ValidateNoActiveProfileAsync(Guid userId)
    {
        if (await _internProfileRepository.HasActiveProfileAsync(userId))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.UserAlreadyHasActiveInternProfile);
        }
    }
}
