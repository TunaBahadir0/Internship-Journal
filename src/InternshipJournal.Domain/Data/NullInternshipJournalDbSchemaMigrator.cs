using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace InternshipJournal.Data;

/* This is used if database provider does't define
 * IInternshipJournalDbSchemaMigrator implementation.
 */
public class NullInternshipJournalDbSchemaMigrator : IInternshipJournalDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
