using System;
using System.Collections.Generic;
using System.Text;
using InternshipJournal.Localization;
using Volo.Abp.Application.Services;

namespace InternshipJournal;

/* Inherit your application services from this class.
 */
public abstract class InternshipJournalAppService : ApplicationService
{
    protected InternshipJournalAppService()
    {
        LocalizationResource = typeof(InternshipJournalResource);
    }
}
