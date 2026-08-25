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

    /// <summary>Belirtilen çalışma yeri bulunamadı.</summary>
    public const string InternProfileWorkplaceNotFound = "InternshipJournal:InternProfileWorkplaceNotFound";

    /// <summary>Pasif bir çalışma yeri staj profiline atanamaz.</summary>
    public const string InternProfileWorkplaceInactive = "InternshipJournal:InternProfileWorkplaceInactive";

    /// <summary>Belirtilen mentor kullanıcısı bulunamadı.</summary>
    public const string InternProfileMentorNotFound = "InternshipJournal:InternProfileMentorNotFound";

    /// <summary>Mentor, stajyerin kendisiyle aynı kullanıcı olamaz.</summary>
    public const string MentorCannotBeSameAsIntern = "InternshipJournal:MentorCannotBeSameAsIntern";

    /// <summary>Kullanıcının zaten aktif bir staj profili var.</summary>
    public const string UserAlreadyHasActiveInternProfile = "InternshipJournal:UserAlreadyHasActiveInternProfile";

    /// <summary>Yalnızca taslak durumundaki profil başlatılabilir.</summary>
    public const string InternProfileCannotBeStarted = "InternshipJournal:InternProfileCannotBeStarted";

    /// <summary>Yalnızca aktif durumdaki profil tamamlanabilir.</summary>
    public const string InternProfileCannotBeCompleted = "InternshipJournal:InternProfileCannotBeCompleted";

    /// <summary>Tamamlanmış bir profil iptal edilemez.</summary>
    public const string InternProfileCannotBeCancelled = "InternshipJournal:InternProfileCannotBeCancelled";

    /// <summary>Gerekli iş günü sayısı pozitif olmalıdır.</summary>
    public const string RequiredWorkDaysMustBePositive = "InternshipJournal:RequiredWorkDaysMustBePositive";

    /// <summary>Gelecek tarihli günlük oluşturulamaz.</summary>
    public const string DailyLogDateCannotBeInFuture = "InternshipJournal:DailyLogDateCannotBeInFuture";

    /// <summary>Günlük tarihi staj dönemi dışında.</summary>
    public const string DailyLogDateOutsideInternshipPeriod = "InternshipJournal:DailyLogDateOutsideInternshipPeriod";

    /// <summary>Belirtilen staj profili bulunamadı.</summary>
    public const string DailyLogInternProfileNotFound = "InternshipJournal:DailyLogInternProfileNotFound";

    /// <summary>Staj profili aktif durumda değil.</summary>
    public const string DailyLogInternProfileNotActive = "InternshipJournal:DailyLogInternProfileNotActive";

    /// <summary>Günlükte en az bir çalışma maddesi bulunmalıdır.</summary>
    public const string DailyLogMustHaveAtLeastOneItem = "InternshipJournal:DailyLogMustHaveAtLeastOneItem";

    /// <summary>Belirtilen çalışma maddesi bulunamadı.</summary>
    public const string DailyLogItemNotFound = "InternshipJournal:DailyLogItemNotFound";

    /// <summary>Çalışma maddesi süresi pozitif olmalıdır.</summary>
    public const string DailyLogItemDurationMustBePositive = "InternshipJournal:DailyLogItemDurationMustBePositive";

    /// <summary>Belirtilen yetkinlik kaydı günlükte bulunamadı.</summary>
    public const string DailyLogSkillEntryNotFound = "InternshipJournal:DailyLogSkillEntryNotFound";

    /// <summary>Belirtilen yetkinlik bulunamadı.</summary>
    public const string DailyLogReferencedSkillNotFound = "InternshipJournal:DailyLogReferencedSkillNotFound";

    /// <summary>Pasif bir yetkinlik günlüğe eklenemez.</summary>
    public const string DailyLogReferencedSkillInactive = "InternshipJournal:DailyLogReferencedSkillInactive";

    /// <summary>Belirtilen problem çözme kaydı bulunamadı.</summary>
    public const string DailyLogProblemEntryNotFound = "InternshipJournal:DailyLogProblemEntryNotFound";

    /// <summary>Yalnızca gönderilmiş günlük onaylanabilir.</summary>
    public const string DailyLogCannotBeApproved = "InternshipJournal:DailyLogCannotBeApproved";

    /// <summary>Yalnızca gönderilmiş günlük için düzeltme istenebilir.</summary>
    public const string DailyLogCannotRequestRevision = "InternshipJournal:DailyLogCannotRequestRevision";

    /// <summary>Yalnızca düzeltme istenen günlük taslağa döndürülebilir.</summary>
    public const string DailyLogCannotReturnToDraft = "InternshipJournal:DailyLogCannotReturnToDraft";

    /// <summary>AI kullanıldıysa araç adı ve kullanım özeti girilmelidir.</summary>
    public const string ProblemSolvingAiToolAndSummaryRequired = "InternshipJournal:ProblemSolvingAiToolAndSummaryRequired";

    /// <summary>AI önerisi reddedildiyse ret gerekçesi girilmelidir.</summary>
    public const string ProblemSolvingAiRejectionReasonRequired = "InternshipJournal:ProblemSolvingAiRejectionReasonRequired";

    /// <summary>Düzeltme talebinde yorum zorunludur.</summary>
    public const string MentorReviewCommentRequiredForRevision = "InternshipJournal:MentorReviewCommentRequiredForRevision";

    /// <summary>Mentor yalnızca kendisine bağlı stajyerin günlüğünü inceleyebilir.</summary>
    public const string MentorReviewNotAuthorized = "InternshipJournal:MentorReviewNotAuthorized";

    /// <summary>İncelenmek istenen günlük bulunamadı.</summary>
    public const string MentorReviewDailyLogNotFound = "InternshipJournal:MentorReviewDailyLogNotFound";
}
