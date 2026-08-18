using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.EntityFrameworkCore;
using InternshipJournal.Locations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace InternshipJournal.Workplaces;

public class WorkplaceRepository : EfCoreRepository<InternshipJournalDbContext, Workplace, Guid>, IWorkplaceRepository
{
    public WorkplaceRepository(IDbContextProvider<InternshipJournalDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Workplace?> FindByNameAsync(string name, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.FirstOrDefaultAsync(x =>
            x.Name == name && (excludedId == null || x.Id != excludedId));
    }

    public async Task<bool> IsNameInUseAsync(string name, Guid? excludedId = null)
    {
        return await FindByNameAsync(name, excludedId) != null;
    }

    public async Task<WorkplaceWithLocation?> GetWithLocationAsync(Guid id)
    {
        var query = await GetWithLocationQueryableAsync();

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<WorkplaceWithLocation>> GetListWithLocationAsync(
        string? filter = null,
        bool? isActive = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue)
    {
        var query = await GetWithLocationQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.Name.Contains(filter));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    private async Task<IQueryable<WorkplaceWithLocation>> GetWithLocationQueryableAsync()
    {
        var dbContext = await GetDbContextAsync();

        return
            from workplace in dbContext.Set<Workplace>()
            join district in dbContext.Set<District>() on workplace.DistrictId equals district.Id
            join province in dbContext.Set<Province>() on district.ProvinceId equals province.Id
            join country in dbContext.Set<Country>() on province.CountryId equals country.Id
            select new WorkplaceWithLocation
            {
                Id = workplace.Id,
                Name = workplace.Name,
                TaxNumber = workplace.TaxNumber,
                Phone = workplace.Phone,
                Email = workplace.Email,
                Website = workplace.Website,
                DistrictId = district.Id,
                DistrictName = district.Name,
                ProvinceId = province.Id,
                ProvinceName = province.Name,
                CountryId = country.Id,
                CountryName = country.Name,
                AddressLine = workplace.AddressLine,
                PostalCode = workplace.PostalCode,
                Latitude = workplace.Latitude,
                Longitude = workplace.Longitude,
                IsActive = workplace.IsActive
            };
    }
}
