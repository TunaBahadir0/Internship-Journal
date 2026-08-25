using InternshipJournal.Localization;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace InternshipJournal.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class InternshipJournalPageModel : AbpPageModel
{
    protected InternshipJournalPageModel()
    {
        LocalizationResourceType = typeof(InternshipJournalResource);
    }

    /// <summary>
    /// Bir BusinessException'ı kullanıcıya gösterilecek metne çevirir. Hata koduna (ex.Code)
    /// karşılık gelen bir çeviri anahtarı varsa onu kullanır; yoksa (ör. başka bir modülden
    /// gelen bir istisna) ex.Message'a geri döner. Bu sayede "Exception of type ... was thrown"
    /// gibi ham CLR mesajları yerine her zaman anlaşılır bir Türkçe/İngilizce açıklama gösterilir.
    /// </summary>
    protected string GetErrorMessage(BusinessException ex)
    {
        if (!string.IsNullOrEmpty(ex.Code))
        {
            var localized = L[ex.Code];
            if (!localized.ResourceNotFound)
            {
                return localized.Value;
            }
        }

        return ex.Message;
    }
}
