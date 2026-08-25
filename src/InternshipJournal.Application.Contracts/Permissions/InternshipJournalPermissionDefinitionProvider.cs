using InternshipJournal.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace InternshipJournal.Permissions;

public class InternshipJournalPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(InternshipJournalPermissions.GroupName, L("Permission:InternshipJournal"));

        var workplaces = myGroup.AddPermission(InternshipJournalPermissions.Workplaces.Default, L("Permission:Workplaces"));
        workplaces.AddChild(InternshipJournalPermissions.Workplaces.Create, L("Permission:Workplaces.Create"));
        workplaces.AddChild(InternshipJournalPermissions.Workplaces.Edit, L("Permission:Workplaces.Edit"));

        var dailyLogs = myGroup.AddPermission(InternshipJournalPermissions.DailyLogs.Default, L("Permission:DailyLogs"));
        dailyLogs.AddChild(InternshipJournalPermissions.DailyLogs.Create, L("Permission:DailyLogs.Create"));
        dailyLogs.AddChild(InternshipJournalPermissions.DailyLogs.Edit, L("Permission:DailyLogs.Edit"));
        dailyLogs.AddChild(InternshipJournalPermissions.DailyLogs.Submit, L("Permission:DailyLogs.Submit"));

        var reviews = myGroup.AddPermission(InternshipJournalPermissions.Reviews.Default, L("Permission:Reviews"));
        reviews.AddChild(InternshipJournalPermissions.Reviews.Approve, L("Permission:Reviews.Approve"));
        reviews.AddChild(InternshipJournalPermissions.Reviews.RequestRevision, L("Permission:Reviews.RequestRevision"));

        myGroup.AddPermission(InternshipJournalPermissions.Reports, L("Permission:Reports"));
        myGroup.AddPermission(InternshipJournalPermissions.Administration, L("Permission:Administration"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<InternshipJournalResource>(name);
    }
}
