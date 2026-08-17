using System.Threading.Tasks;

namespace InternshipJournal.Data;

public interface IInternshipJournalDbSchemaMigrator
{
    Task MigrateAsync();
}
