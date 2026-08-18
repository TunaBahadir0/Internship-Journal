using System;
using System.ComponentModel.DataAnnotations;
using InternshipJournal.Consts;

namespace InternshipJournal.Workplaces;

public class UpdateWorkplaceDto
{
    [Required]
    [StringLength(WorkplaceConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [StringLength(WorkplaceConsts.MaxTaxNumberLength)]
    public string? TaxNumber { get; set; }

    [StringLength(WorkplaceConsts.MaxPhoneLength)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(WorkplaceConsts.MaxEmailLength)]
    public string? Email { get; set; }

    [StringLength(WorkplaceConsts.MaxWebsiteLength)]
    public string? Website { get; set; }

    [Required]
    public Guid DistrictId { get; set; }

    [Required]
    [StringLength(WorkplaceConsts.MaxAddressLineLength)]
    public string AddressLine { get; set; } = null!;

    [StringLength(WorkplaceConsts.MaxPostalCodeLength)]
    public string? PostalCode { get; set; }

    [Range(typeof(decimal), "-90", "90")]
    public decimal? Latitude { get; set; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal? Longitude { get; set; }
}
