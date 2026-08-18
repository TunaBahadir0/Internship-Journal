using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace InternshipJournal.InternProfiles;

public class DateRange : ValueObject
{
    public DateTime StartDate { get; }

    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate.Date < startDate.Date)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InvalidDateRange);
        }

        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public bool Contains(DateTime date)
    {
        return date.Date >= StartDate && date.Date <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public int DurationInDays()
    {
        return (EndDate - StartDate).Days + 1;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
