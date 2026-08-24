using System;
using System.Threading.Tasks;
using InternshipJournal.Enums;
using InternshipJournal.InternProfiles;
using InternshipJournal.Skills;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Timing;
using Xunit;

namespace InternshipJournal.DailyLogs;

public class DailyLogManagerTests
{
    private static readonly DateTime Today = new(2026, 8, 15);

    private static InternProfile CreateActiveInternProfile(DateRange? period = null)
    {
        var profile = new InternProfile(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Üniversitesi",
            "Yazılım Mühendisliği",
            "12345678",
            period ?? new DateRange(new DateTime(2026, 8, 1), new DateTime(2026, 9, 30)),
            60);
        profile.Start();
        return profile;
    }

    private static DailyLogManager CreateManager(
        out IDailyLogRepository dailyLogRepository,
        out IInternProfileRepository internProfileRepository,
        out Volo.Abp.Domain.Repositories.IRepository<Skill, Guid> skillRepository)
    {
        dailyLogRepository = Substitute.For<IDailyLogRepository>();
        internProfileRepository = Substitute.For<IInternProfileRepository>();
        skillRepository = Substitute.For<Volo.Abp.Domain.Repositories.IRepository<Skill, Guid>>();

        var clock = Substitute.For<IClock>();
        clock.Now.Returns(Today);

        dailyLogRepository.ExistsForDateAsync(Arg.Any<Guid>(), Arg.Any<DateTime>()).Returns(false);

        return new DailyLogManager(dailyLogRepository, internProfileRepository, skillRepository, clock);
    }

    [Fact]
    public async Task Create_WhenValid_ShouldCreate()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository, out _);
        var profile = CreateActiveInternProfile();
        internProfileRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(profile);

        var log = await manager.CreateAsync(profile.Id, new DateTime(2026, 8, 10), "Test günlüğü");

        log.InternProfileId.ShouldBe(profile.Id);
        log.LogDate.ShouldBe(new DateTime(2026, 8, 10));
    }

    [Fact]
    public async Task Create_WhenInternProfileNotFound_ShouldFail()
    {
        var manager = CreateManager(out _, out var internProfileRepository, out _);
        internProfileRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs((InternProfile?)null);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.CreateAsync(Guid.NewGuid(), new DateTime(2026, 8, 10), null));
    }

    [Fact]
    public async Task Create_WhenInternProfileNotActive_ShouldFail()
    {
        var manager = CreateManager(out _, out var internProfileRepository, out _);
        var profile = CreateActiveInternProfile();
        profile.Complete();
        internProfileRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(profile);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.CreateAsync(profile.Id, new DateTime(2026, 8, 10), null));
    }

    [Fact]
    public async Task Create_WhenDateInFuture_ShouldFail()
    {
        var manager = CreateManager(out _, out var internProfileRepository, out _);
        var profile = CreateActiveInternProfile();
        internProfileRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(profile);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.CreateAsync(profile.Id, Today.AddDays(1), null));
    }

    [Fact]
    public async Task Create_WhenDateOutsideInternshipPeriod_ShouldFail()
    {
        var manager = CreateManager(out _, out var internProfileRepository, out _);
        var profile = CreateActiveInternProfile(new DateRange(new DateTime(2026, 8, 1), new DateTime(2026, 8, 10)));
        internProfileRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(profile);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.CreateAsync(profile.Id, new DateTime(2026, 8, 12), null));
    }

    [Fact]
    public async Task Create_WhenLogExistsForDate_ShouldFail()
    {
        var manager = CreateManager(out var dailyLogRepository, out var internProfileRepository, out _);
        var profile = CreateActiveInternProfile();
        internProfileRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(profile);
        dailyLogRepository.ExistsForDateAsync(Arg.Any<Guid>(), Arg.Any<DateTime>()).Returns(true);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.CreateAsync(profile.Id, new DateTime(2026, 8, 10), null));
    }

    [Fact]
    public async Task AddSkill_WhenSkillInactive_ShouldFail()
    {
        var manager = CreateManager(out _, out _, out var skillRepository);
        var log = new DailyLog(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 8, 10), null);
        var skill = new Skill(Guid.NewGuid(), "C#", isActive: false);
        skillRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(skill);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.AddSkillAsync(log, skill.Id, LearningLevel.Practiced, null));
    }

    [Fact]
    public async Task AddSkill_WhenSkillNotFound_ShouldFail()
    {
        var manager = CreateManager(out _, out _, out var skillRepository);
        var log = new DailyLog(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 8, 10), null);
        skillRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs((Skill?)null);

        await Assert.ThrowsAsync<BusinessException>(() =>
            manager.AddSkillAsync(log, Guid.NewGuid(), LearningLevel.Practiced, null));
    }

    [Fact]
    public async Task AddSkill_WhenSkillActive_ShouldAdd()
    {
        var manager = CreateManager(out _, out _, out var skillRepository);
        var log = new DailyLog(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 8, 10), null);
        var skill = new Skill(Guid.NewGuid(), "C#", isActive: true);
        skillRepository.FindAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(skill);

        await manager.AddSkillAsync(log, skill.Id, LearningLevel.Practiced, "Not");

        log.Skills.Count.ShouldBe(1);
    }
}
