using System;
using System.Net.Mail;
using InternshipJournal.Consts;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InternshipJournal.Workplaces;

public class Workplace : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;

    public string? TaxNumber { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Website { get; private set; }

    public Guid DistrictId { get; private set; }

    public string AddressLine { get; private set; } = null!;

    public string? PostalCode { get; private set; }

    public decimal? Latitude { get; private set; }

    public decimal? Longitude { get; private set; }

    public bool IsActive { get; private set; }

    protected Workplace()
    {
        /* Required by EF Core. */
    }

    internal Workplace(
        Guid id,
        [NotNull] string name,
        Guid districtId,
        [NotNull] string addressLine,
        [CanBeNull] string? postalCode = null,
        [CanBeNull] string? taxNumber = null,
        [CanBeNull] string? phone = null,
        [CanBeNull] string? email = null,
        [CanBeNull] string? website = null,
        decimal? latitude = null,
        decimal? longitude = null,
        bool isActive = true) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), WorkplaceConsts.MaxNameLength);
        DistrictId = districtId;
        AddressLine = Check.NotNullOrWhiteSpace(addressLine, nameof(addressLine), WorkplaceConsts.MaxAddressLineLength);
        PostalCode = Check.Length(postalCode, nameof(postalCode), WorkplaceConsts.MaxPostalCodeLength);
        TaxNumber = Check.Length(taxNumber, nameof(taxNumber), WorkplaceConsts.MaxTaxNumberLength);
        Phone = Check.Length(phone, nameof(phone), WorkplaceConsts.MaxPhoneLength);
        SetEmail(email);
        Website = Check.Length(website, nameof(website), WorkplaceConsts.MaxWebsiteLength);
        SetCoordinates(latitude, longitude);
        IsActive = isActive;
    }

    public void Rename([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), WorkplaceConsts.MaxNameLength);
    }

    public void ChangeContactInformation(
        [CanBeNull] string? taxNumber,
        [CanBeNull] string? phone,
        [CanBeNull] string? email,
        [CanBeNull] string? website)
    {
        TaxNumber = Check.Length(taxNumber, nameof(taxNumber), WorkplaceConsts.MaxTaxNumberLength);
        Phone = Check.Length(phone, nameof(phone), WorkplaceConsts.MaxPhoneLength);
        SetEmail(email);
        Website = Check.Length(website, nameof(website), WorkplaceConsts.MaxWebsiteLength);
    }

    public void ChangeAddress(Guid districtId, [NotNull] string addressLine, [CanBeNull] string? postalCode)
    {
        DistrictId = districtId;
        AddressLine = Check.NotNullOrWhiteSpace(addressLine, nameof(addressLine), WorkplaceConsts.MaxAddressLineLength);
        PostalCode = Check.Length(postalCode, nameof(postalCode), WorkplaceConsts.MaxPostalCodeLength);
    }

    public void ChangeCoordinates(decimal? latitude, decimal? longitude)
    {
        SetCoordinates(latitude, longitude);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetEmail([CanBeNull] string? email)
    {
        email = Check.Length(email, nameof(email), WorkplaceConsts.MaxEmailLength);

        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InvalidWorkplaceEmailFormat);
        }

        Email = email;
    }

    private void SetCoordinates(decimal? latitude, decimal? longitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InvalidWorkplaceLatitude);
        }

        if (longitude is < -180 or > 180)
        {
            throw new BusinessException(InternshipJournalDomainErrorCodes.InvalidWorkplaceLongitude);
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
