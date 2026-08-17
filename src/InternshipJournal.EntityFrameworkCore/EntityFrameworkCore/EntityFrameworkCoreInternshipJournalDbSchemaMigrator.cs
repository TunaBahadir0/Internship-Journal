using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InternshipJournal.Data;
using Volo.Abp.DependencyInjection;

namespace InternshipJournal.EntityFrameworkCore;

public class EntityFrameworkCoreInternshipJournalDbSchemaMigrator
    : IInternshipJournalDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreInternshipJournalDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the InternshipJournalDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<InternshipJournalDbContext>()
            .Database
            .MigrateAsync();
    }
}
