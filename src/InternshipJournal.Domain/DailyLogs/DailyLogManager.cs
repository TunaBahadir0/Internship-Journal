using System;
using System.Threading.Tasks;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using InternshipJournal.Skills;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Timing;

namespace InternshipJournal.DailyLogs;

public class DailyLogManager : DomainService
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IInternProfileRepository _internProfileRepository;
    private readonly IRepository<Skill, Guid> _skillRepository;
    private readonly IClock _clock;

    public DailyLogManager(
        IDailyLogRepository dailyLogRepository,
        IInternProfileRepository internProfileRepository,
        IRepository<Skill, Guid> skillRepository,
        IClock clock)
    {
        _dailyLogRepository = dailyLogRepository;
        _internProfileRepository = internProfileRepository;
        _skillRepository = skillRepository;
        _clock = clock;
    }

    public async Task<DailyLog> CreateAsync(
        Guid internProfileId,
        DateTime logDate,
        string? summary)
    {
        var internProfile = await GetActiveInternProfileAsync(internProfileId);

        var date = logDate.Date;

        if (date > _clock.Now.Date)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogDateCannotBeInFuture);
        }

        if (!internProfile.InternshipPeriod.Contains(date))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogDateOutsideInternshipPeriod);
        }

        if (await _dailyLogRepository.ExistsForDateAsync(internProfileId, date))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DuplicateDailyLog);
        }

        return new DailyLog(Guid.NewGuid(), internProfileId, date, summary);
    }

    public async Task AddSkillAsync(
        DailyLog dailyLog,
        Guid skillId,
        LearningLevel learningLevel,
        string? note)
    {
        var skill = await _skillRepository.FindAsync(skillId);
        if (skill == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogReferencedSkillNotFound);
        }

        if (!skill.IsActive)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogReferencedSkillInactive);
        }

        dailyLog.AddSkill(skillId, learningLevel, note);
    }

    private async Task<InternProfile> GetActiveInternProfileAsync(Guid internProfileId)
    {
        var internProfile = await _internProfileRepository.FindAsync(internProfileId);
        if (internProfile == null)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogInternProfileNotFound);
        }

        if (internProfile.Status != InternshipStatus.Active)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.DailyLogInternProfileNotActive);
        }

        return internProfile;
    }
}
