using System;
using System.Linq;
using InternshipJournal.Enums;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace InternshipJournal.DailyLogs;

public class DailyLogTests
{
    private static DailyLog CreateLog()
    {
        return new DailyLog(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 8, 10), "Test günlüğü");
    }

    private static DailyLog CreateLogWithOneItem()
    {
        var log = CreateLog();
        log.AddItem("Ortam kurulumu", "Docker ile PostgreSQL kuruldu.", WorkType.Setup, 60, true);
        return log;
    }

    [Fact]
    public void AddItem_WhenEditable_ShouldAdd()
    {
        var log = CreateLog();

        log.AddItem("Domain katmanı geliştirme", "DailyLog aggregate'i yazıldı.", WorkType.Development, 120, true);

        log.Items.Count.ShouldBe(1);
        log.TotalMinutes.ShouldBe(120);
    }

    [Fact]
    public void AddItem_WhenApproved_ShouldFail()
    {
        var log = CreateLogWithOneItem();
        log.Submit();
        log.Approve();

        Assert.Throws<BusinessException>(() =>
            log.AddItem("Geç eklenen madde", null, WorkType.Development, 30, true));
    }

    [Fact]
    public void AddItem_WhenDurationNotPositive_ShouldFail()
    {
        var log = CreateLog();

        Assert.Throws<BusinessException>(() =>
            log.AddItem("Geçersiz süre", null, WorkType.Development, 0, false));
    }

    [Fact]
    public void UpdateItem_ShouldRecalculateTotalMinutes()
    {
        var log = CreateLogWithOneItem();
        var itemId = log.Items.First().Id;

        log.UpdateItem(itemId, "Ortam kurulumu (güncel)", null, WorkType.Setup, 90, true);

        log.TotalMinutes.ShouldBe(90);
    }

    [Fact]
    public void RemoveItem_ShouldRecalculateTotalMinutes()
    {
        var log = CreateLogWithOneItem();
        log.AddItem("İkinci madde", null, WorkType.Development, 45, true);
        var firstItemId = log.Items.First().Id;

        log.RemoveItem(firstItemId);

        log.Items.Count.ShouldBe(1);
        log.TotalMinutes.ShouldBe(45);
    }

    [Fact]
    public void AddSkill_WhenDuplicate_ShouldFail()
    {
        var log = CreateLog();
        var skillId = Guid.NewGuid();
        log.AddSkill(skillId, LearningLevel.Practiced, "İlk not");

        Assert.Throws<BusinessException>(() =>
            log.AddSkill(skillId, LearningLevel.Applied, "İkinci not"));
    }

    [Fact]
    public void AddProblem_WhenAiUsedWithoutNote_ShouldFail()
    {
        var log = CreateLog();

        Assert.Throws<BusinessException>(() =>
            log.AddProblem(
                "Migration hatası",
                "EF Core migration çalışmadı.",
                "42P01: relation does not exist",
                "Migration'ı sildim, tekrar oluşturdum.",
                "Eksik DbSet tanımı.",
                "DbSet eklenip migration yeniden üretildi.",
                usedArtificialIntelligence: true,
                aiToolName: null,
                aiPromptSummary: null,
                aiSuggestion: null,
                aiSuggestionAccepted: null,
                aiRejectionReason: null));
    }

    [Fact]
    public void Submit_WhenNoItem_ShouldFail()
    {
        var log = CreateLog();

        Assert.Throws<BusinessException>(() => log.Submit());
    }

    [Fact]
    public void Submit_WhenValid_ShouldSubmit()
    {
        var log = CreateLogWithOneItem();

        log.Submit();

        log.Status.ShouldBe(DailyLogStatus.Submitted);
        log.SubmittedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Submit_WhenAlreadySubmitted_ShouldFail()
    {
        var log = CreateLogWithOneItem();
        log.Submit();

        Assert.Throws<BusinessException>(() => log.Submit());
    }

    [Fact]
    public void Approve_WhenSubmitted_ShouldApprove()
    {
        var log = CreateLogWithOneItem();
        log.Submit();

        log.Approve();

        log.Status.ShouldBe(DailyLogStatus.Approved);
        log.ApprovedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Approve_WhenDraft_ShouldFail()
    {
        var log = CreateLogWithOneItem();

        Assert.Throws<BusinessException>(() => log.Approve());
    }

    [Fact]
    public void RequestRevision_WhenSubmitted_ShouldChangeStatus()
    {
        var log = CreateLogWithOneItem();
        log.Submit();

        log.RequestRevision();

        log.Status.ShouldBe(DailyLogStatus.RevisionRequested);
        log.ReviewedAt.ShouldNotBeNull();
    }

    [Fact]
    public void ReturnToDraft_WhenRevisionRequested_ShouldReturnToDraft()
    {
        var log = CreateLogWithOneItem();
        log.Submit();
        log.RequestRevision();

        log.ReturnToDraft();

        log.Status.ShouldBe(DailyLogStatus.Draft);
    }

    [Fact]
    public void ReturnToDraft_WhenDraft_ShouldFail()
    {
        var log = CreateLog();

        Assert.Throws<BusinessException>(() => log.ReturnToDraft());
    }
}
