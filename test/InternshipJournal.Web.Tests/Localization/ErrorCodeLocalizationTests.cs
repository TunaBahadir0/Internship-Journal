using System.Linq;
using System.Reflection;
using InternshipJournal;
using Microsoft.Extensions.Localization;
using Shouldly;
using Volo.Abp.Localization;
using Xunit;

namespace InternshipJournal.Localization;

// Yönetici geri bildirimi: BusinessException'lar kullanıcıya "Exception of type ... was thrown"
// gibi ham CLR mesajlarıyla değil, anlaşılır bir açıklamayla gösterilmeli
// (bkz. InternshipJournalPageModel.GetErrorMessage). Bu test, InternshipJournalDomainErrorCodes'daki
// her kodun tr ve en kaynaklarında gerçekten bir çeviri karşılığı olduğunu garanti eder — yeni bir
// hata kodu eklenip çevirisi unutulursa bu test kırılır.
public class ErrorCodeLocalizationTests : InternshipJournalWebTestBase
{
    [Theory]
    [InlineData("tr")]
    [InlineData("en")]
    public void AllErrorCodes_ShouldHaveLocalizedMessage(string culture)
    {
        var localizer = GetRequiredService<IStringLocalizer<InternshipJournalResource>>();

        var errorCodes = typeof(InternshipJournalDomainErrorCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.FieldType == typeof(string))
            .Select(x => (string)x.GetValue(null)!)
            .ToList();

        errorCodes.ShouldNotBeEmpty();

        using (CultureHelper.Use(culture))
        {
            foreach (var code in errorCodes)
            {
                var localized = localizer[code];
                localized.ResourceNotFound.ShouldBeFalse($"'{code}' için '{culture}' kültüründe çeviri bulunamadı.");
                localized.Value.ShouldNotBeNullOrWhiteSpace();
            }
        }
    }
}
