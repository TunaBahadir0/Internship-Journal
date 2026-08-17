using InternshipJournal.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace InternshipJournal.Permissions;

public class InternshipJournalPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(InternshipJournalPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(InternshipJournalPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<InternshipJournalResource>(name);
    }
}
