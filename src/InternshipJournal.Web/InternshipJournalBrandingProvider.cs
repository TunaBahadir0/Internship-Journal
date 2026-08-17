using Microsoft.Extensions.Localization;
using InternshipJournal.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace InternshipJournal.Web;

[Dependency(ReplaceServices = true)]
public class InternshipJournalBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<InternshipJournalResource> _localizer;

    public InternshipJournalBrandingProvider(IStringLocalizer<InternshipJournalResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
