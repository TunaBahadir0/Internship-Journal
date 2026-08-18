using System;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace InternshipJournal.InternProfiles;

public class DateRangeTests
{
    [Fact]
    public void Create_WhenDatesValid_ShouldCreate()
    {
        var range = new DateRange(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        range.StartDate.ShouldBe(new DateTime(2026, 1, 1));
        range.EndDate.ShouldBe(new DateTime(2026, 1, 31));
    }

    [Fact]
    public void Create_WhenEndBeforeStart_ShouldFail()
    {
        Assert.Throws<BusinessException>(() =>
            new DateRange(new DateTime(2026, 1, 31), new DateTime(2026, 1, 1)));
    }

    [Fact]
    public void Contains_WhenDateInside_ShouldReturnTrue()
    {
        var range = new DateRange(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        range.Contains(new DateTime(2026, 1, 15)).ShouldBeTrue();
    }

    [Fact]
    public void Contains_WhenDateOutside_ShouldReturnFalse()
    {
        var range = new DateRange(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        range.Contains(new DateTime(2026, 2, 1)).ShouldBeFalse();
    }

    [Fact]
    public void Overlaps_WhenRangesIntersect_ShouldReturnTrue()
    {
        var range1 = new DateRange(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));
        var range2 = new DateRange(new DateTime(2026, 1, 20), new DateTime(2026, 2, 10));

        range1.Overlaps(range2).ShouldBeTrue();
    }

    [Fact]
    public void Overlaps_WhenRangesDoNotIntersect_ShouldReturnFalse()
    {
        var range1 = new DateRange(new DateTime(2026, 1, 1), new DateTime(2026, 1, 10));
        var range2 = new DateRange(new DateTime(2026, 2, 1), new DateTime(2026, 2, 10));

        range1.Overlaps(range2).ShouldBeFalse();
    }

    [Fact]
    public void DurationInDays_ShouldIncludeBothEndpoints()
    {
        var range = new DateRange(new DateTime(2026, 1, 1), new DateTime(2026, 1, 10));

        range.DurationInDays().ShouldBe(10);
    }
}
