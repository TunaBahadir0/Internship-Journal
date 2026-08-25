namespace InternshipJournal.Permissions;

public static class InternshipJournalPermissions
{
    public const string GroupName = "InternshipJournal";

    public static class Workplaces
    {
        public const string Default = GroupName + ".Workplaces";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
    }

    public static class DailyLogs
    {
        public const string Default = GroupName + ".DailyLogs";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Submit = Default + ".Submit";
    }

    public static class Reviews
    {
        public const string Default = GroupName + ".Reviews";
        public const string Approve = Default + ".Approve";
        public const string RequestRevision = Default + ".RequestRevision";
    }

    public const string Reports = GroupName + ".Reports";
    public const string Administration = GroupName + ".Administration";
}
