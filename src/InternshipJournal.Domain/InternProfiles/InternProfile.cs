using System;
using InternshipJournal.Consts;
using InternshipJournal.Enums;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.InternProfiles;

public class InternProfile : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    public Guid MentorUserId { get; private set; }

    public Guid WorkplaceId { get; private set; }

    public string University { get; private set; } = null!;

    public string SchoolDepartment { get; private set; } = null!;

    public string StudentNumber { get; private set; } = null!;

    public DateRange InternshipPeriod { get; private set; } = null!;

    public int RequiredWorkDays { get; private set; }

    public InternshipStatus Status { get; private set; }

    protected InternProfile()
    {
        /* Required by EF Core. */
    }

    internal InternProfile(
        Guid id,
        Guid userId,
        Guid mentorUserId,
        Guid workplaceId,
        [NotNull] string university,
        [NotNull] string schoolDepartment,
        [NotNull] string studentNumber,
        [NotNull] DateRange internshipPeriod,
        int requiredWorkDays) : base(id)
    {
        UserId = userId;
        WorkplaceId = workplaceId;
        SetMentor(mentorUserId);
        SetEducationInformation(university, schoolDepartment, studentNumber);
        InternshipPeriod = Check.NotNull(internshipPeriod, nameof(internshipPeriod));
        SetRequiredWorkDays(requiredWorkDays);
        Status = InternshipStatus.Draft;
    }

    public void ChangeMentor(Guid mentorUserId)
    {
        SetMentor(mentorUserId);
    }

    public void ChangeWorkplace(Guid workplaceId)
    {
        WorkplaceId = workplaceId;
    }

    public void ChangeEducationInformation(
        [NotNull] string university,
        [NotNull] string schoolDepartment,
        [NotNull] string studentNumber)
    {
        SetEducationInformation(university, schoolDepartment, studentNumber);
    }

    public void ChangeInternshipPeriod([NotNull] DateRange period)
    {
        InternshipPeriod = Check.NotNull(period, nameof(period));
    }

    public void ChangeRequiredWorkDays(int requiredWorkDays)
    {
        SetRequiredWorkDays(requiredWorkDays);
    }

    public void Start()
    {
        if (Status != InternshipStatus.Draft)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InternProfileCannotBeStarted);
        }

        Status = InternshipStatus.Active;
    }

    public void Complete()
    {
        if (Status != InternshipStatus.Active)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InternProfileCannotBeCompleted);
        }

        Status = InternshipStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == InternshipStatus.Completed)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InternProfileCannotBeCancelled);
        }

        Status = InternshipStatus.Cancelled;
    }

    private void SetMentor(Guid mentorUserId)
    {
        if (mentorUserId == UserId)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.MentorCannotBeSameAsIntern);
        }

        MentorUserId = mentorUserId;
    }

    private void SetEducationInformation(
        [NotNull] string university,
        [NotNull] string schoolDepartment,
        [NotNull] string studentNumber)
    {
        University = Check.NotNullOrWhiteSpace(university, nameof(university), InternProfileConsts.MaxUniversityLength);
        SchoolDepartment = Check.NotNullOrWhiteSpace(schoolDepartment, nameof(schoolDepartment), InternProfileConsts.MaxSchoolDepartmentLength);
        StudentNumber = Check.NotNullOrWhiteSpace(studentNumber, nameof(studentNumber), InternProfileConsts.MaxStudentNumberLength);
    }

    private void SetRequiredWorkDays(int requiredWorkDays)
    {
        if (requiredWorkDays <= 0)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.RequiredWorkDaysMustBePositive);
        }

        RequiredWorkDays = requiredWorkDays;
    }
}
