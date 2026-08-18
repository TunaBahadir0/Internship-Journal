namespace InternshipJournal;

public static class InternshipJournalDomainErrorCodes
{
    /* Format: "InternshipJournal:00000" for the code
     * Norm: "InternshipJournal:Xyz" for the message resource name (Xyz = error code without namespace)
     */

    /// <summary>Bu ülke kodu zaten kullanılıyor.</summary>
    public const string CountryCodeAlreadyExists = "InternshipJournal:CountryCodeAlreadyExists";

    /// <summary>Bu il, seçilen ülkede zaten var.</summary>
    public const string ProvinceAlreadyExists = "InternshipJournal:ProvinceAlreadyExists";

    /// <summary>Bu ilçe, seçilen ilde zaten var.</summary>
    public const string DistrictAlreadyExists = "InternshipJournal:DistrictAlreadyExists";

    /// <summary>Bu yetkinlik adı zaten kayıtlı.</summary>
    public const string SkillNameAlreadyExists = "InternshipJournal:SkillNameAlreadyExists";

    /// <summary>Pasif bir konum (ülke/il/ilçe) seçilemez.</summary>
    public const string InactiveLocationCannotBeSelected = "InternshipJournal:InactiveLocationCannotBeSelected";

    /// <summary>Geçersiz tarih aralığı: bitiş tarihi başlangıçtan önce olamaz.</summary>
    public const string InvalidDateRange = "InternshipJournal:InvalidDateRange";

    /// <summary>Bu tarih için zaten bir günlük mevcut.</summary>
    public const string DuplicateDailyLog = "InternshipJournal:DuplicateDailyLog";

    /// <summary>Bu yetkinlik günlüğe zaten eklenmiş.</summary>
    public const string DuplicateDailyLogSkill = "InternshipJournal:DuplicateDailyLogSkill";

    /// <summary>Gönderilmiş veya onaylanmış günlük düzenlenemez.</summary>
    public const string DailyLogCannotBeEdited = "InternshipJournal:DailyLogCannotBeEdited";

    /// <summary>Günlük şu anki durumda gönderilemez.</summary>
    public const string DailyLogCannotBeSubmitted = "InternshipJournal:DailyLogCannotBeSubmitted";

    /// <summary>Stajyer profiline mentor atanmamış.</summary>
    public const string MentorIsNotAssigned = "InternshipJournal:MentorIsNotAssigned";

    /// <summary>Bu çalışma yeri adı zaten kullanılıyor.</summary>
    public const string WorkplaceNameAlreadyExists = "InternshipJournal:WorkplaceNameAlreadyExists";

    /// <summary>Belirtilen ilçe bulunamadı.</summary>
    public const string WorkplaceDistrictNotFound = "InternshipJournal:WorkplaceDistrictNotFound";

    /// <summary>Girilen e-posta adresi geçerli bir formatta değil.</summary>
    public const string InvalidWorkplaceEmailFormat = "InternshipJournal:InvalidWorkplaceEmailFormat";

    /// <summary>Enlem değeri -90 ile 90 arasında olmalıdır.</summary>
    public const string InvalidWorkplaceLatitude = "InternshipJournal:InvalidWorkplaceLatitude";

    /// <summary>Boylam değeri -180 ile 180 arasında olmalıdır.</summary>
    public const string InvalidWorkplaceLongitude = "InternshipJournal:InvalidWorkplaceLongitude";
}
