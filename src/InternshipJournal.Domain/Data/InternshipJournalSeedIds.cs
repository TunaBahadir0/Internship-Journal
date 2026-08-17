using System;

namespace InternshipJournal.Data;

public static class InternshipJournalSeedIds
{
    public static class Countries
    {
        public static readonly Guid Turkey = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Germany = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid Netherlands = Guid.Parse("10000000-0000-0000-0000-000000000003");
    }

    public static class Provinces
    {
        public static readonly Guid Istanbul = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid Ankara = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Izmir = Guid.Parse("20000000-0000-0000-0000-000000000003");
        public static readonly Guid Bursa = Guid.Parse("20000000-0000-0000-0000-000000000004");
        public static readonly Guid Kocaeli = Guid.Parse("20000000-0000-0000-0000-000000000005");
    }

    public static class Districts
    {
        // İstanbul
        public static readonly Guid Kadikoy = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid Uskudar = Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid Sisli = Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid Besiktas = Guid.Parse("30000000-0000-0000-0000-000000000004");
        public static readonly Guid Atasehir = Guid.Parse("30000000-0000-0000-0000-000000000005");

        // Ankara
        public static readonly Guid Cankaya = Guid.Parse("30000000-0000-0000-0000-000000000006");
        public static readonly Guid Yenimahalle = Guid.Parse("30000000-0000-0000-0000-000000000007");
        public static readonly Guid Kecioren = Guid.Parse("30000000-0000-0000-0000-000000000008");

        // İzmir
        public static readonly Guid Konak = Guid.Parse("30000000-0000-0000-0000-000000000009");
        public static readonly Guid Bornova = Guid.Parse("30000000-0000-0000-0000-00000000000a");
        public static readonly Guid Karsiyaka = Guid.Parse("30000000-0000-0000-0000-00000000000b");
    }

    public static class Skills
    {
        public static readonly Guid CSharp = Guid.Parse("40000000-0000-0000-0000-000000000001");
        public static readonly Guid DotNet = Guid.Parse("40000000-0000-0000-0000-000000000002");
        public static readonly Guid AbpFramework = Guid.Parse("40000000-0000-0000-0000-000000000003");
        public static readonly Guid EntityFrameworkCore = Guid.Parse("40000000-0000-0000-0000-000000000004");
        public static readonly Guid PostgreSql = Guid.Parse("40000000-0000-0000-0000-000000000005");
        public static readonly Guid Docker = Guid.Parse("40000000-0000-0000-0000-000000000006");
        public static readonly Guid Git = Guid.Parse("40000000-0000-0000-0000-000000000007");
        public static readonly Guid Ddd = Guid.Parse("40000000-0000-0000-0000-000000000008");
        public static readonly Guid RazorPages = Guid.Parse("40000000-0000-0000-0000-000000000009");
        public static readonly Guid HtmlCss = Guid.Parse("40000000-0000-0000-0000-00000000000a");
        public static readonly Guid JavaScript = Guid.Parse("40000000-0000-0000-0000-00000000000b");
        public static readonly Guid UnitTesting = Guid.Parse("40000000-0000-0000-0000-00000000000c");
        public static readonly Guid ProblemSolving = Guid.Parse("40000000-0000-0000-0000-00000000000d");
        public static readonly Guid AiAssistedCoding = Guid.Parse("40000000-0000-0000-0000-00000000000e");
    }
}
