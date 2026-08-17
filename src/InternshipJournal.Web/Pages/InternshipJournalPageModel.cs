using InternshipJournal.Localization;
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
}
