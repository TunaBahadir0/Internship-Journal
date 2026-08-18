using System;

namespace InternshipJournal.Workplaces;

public class WorkplaceWithLocation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? TaxNumber { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public Guid DistrictId { get; set; }

    public string DistrictName { get; set; } = null!;

    public Guid ProvinceId { get; set; }

    public string ProvinceName { get; set; } = null!;

    public Guid CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public string AddressLine { get; set; } = null!;

    public string? PostalCode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; }
}
