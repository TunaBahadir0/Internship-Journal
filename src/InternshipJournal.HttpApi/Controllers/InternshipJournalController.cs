using InternshipJournal.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace InternshipJournal.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class InternshipJournalController : AbpControllerBase
{
    protected InternshipJournalController()
    {
        LocalizationResource = typeof(InternshipJournalResource);
    }
}
