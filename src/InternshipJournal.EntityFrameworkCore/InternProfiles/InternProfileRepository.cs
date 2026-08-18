using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternshipJournal.EntityFrameworkCore;
using InternshipJournal.Enums;
using InternshipJournal.Locations;
using InternshipJournal.Workplaces;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace InternshipJournal.InternProfiles;

public class InternProfileRepository : EfCoreRepository<InternshipJournalDbContext, InternProfile, Guid>, IInternProfileRepository
{
    public InternProfileRepository(IDbContextProvider<InternshipJournalDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<InternProfile?> FindByUserIdAsync(Guid userId)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreationTime)
            .FirstOrDefaultAsync();
    }

    public async Task<InternProfile?> GetActiveByUserIdAsync(Guid userId)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == InternshipStatus.Active);
    }

    public async Task<bool> HasActiveProfileAsync(Guid userId)
    {
        return await GetActiveByUserIdAsync(userId) != null;
    }

    public async Task<InternProfileWithWorkplace?> GetWithWorkplaceAsync(Guid id)
    {
        var query = await GetWithWorkplaceQueryableAsync();

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<InternProfileWithDetails?> GetWithMentorAndWorkplaceAsync(Guid id)
    {
        var query = await GetWithMentorAndWorkplaceQueryableAsync();

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<InternProfileWithDetails>> GetListWithDetailsAsync(
        string? filter = null,
        InternshipStatus? status = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue)
    {
        var query = await GetWithMentorAndWorkplaceQueryableAsync();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(x => x.UserName.Contains(filter) || x.FullName.Contains(filter));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.InternshipStartDate)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();
    }

    private async Task<IQueryable<InternProfileWithWorkplace>> GetWithWorkplaceQueryableAsync()
    {
        var dbContext = await GetDbContextAsync();

        return
            from profile in dbContext.Set<InternProfile>()
            join mentor in dbContext.Set<IdentityUser>() on profile.MentorUserId equals mentor.Id
            join workplace in dbContext.Set<Workplace>() on profile.WorkplaceId equals workplace.Id
            join district in dbContext.Set<District>() on workplace.DistrictId equals district.Id
            join province in dbContext.Set<Province>() on district.ProvinceId equals province.Id
            join country in dbContext.Set<Country>() on province.CountryId equals country.Id
            select new InternProfileWithWorkplace
            {
                Id = profile.Id,
                UserId = profile.UserId,
                MentorUserId = mentor.Id,
                MentorFullName = (mentor.Name + " " + mentor.Surname).Trim(),
                WorkplaceId = workplace.Id,
                WorkplaceName = workplace.Name,
                DistrictName = district.Name,
                ProvinceName = province.Name,
                CountryName = country.Name,
                AddressLine = workplace.AddressLine,
                University = profile.University,
                SchoolDepartment = profile.SchoolDepartment,
                StudentNumber = profile.StudentNumber,
                InternshipStartDate = profile.InternshipPeriod.StartDate,
                InternshipEndDate = profile.InternshipPeriod.EndDate,
                RequiredWorkDays = profile.RequiredWorkDays,
                Status = profile.Status
            };
    }

    private async Task<IQueryable<InternProfileWithDetails>> GetWithMentorAndWorkplaceQueryableAsync()
    {
        var dbContext = await GetDbContextAsync();

        return
            from profile in dbContext.Set<InternProfile>()
            join user in dbContext.Set<IdentityUser>() on profile.UserId equals user.Id
            join mentor in dbContext.Set<IdentityUser>() on profile.MentorUserId equals mentor.Id
            join workplace in dbContext.Set<Workplace>() on profile.WorkplaceId equals workplace.Id
            join district in dbContext.Set<District>() on workplace.DistrictId equals district.Id
            join province in dbContext.Set<Province>() on district.ProvinceId equals province.Id
            join country in dbContext.Set<Country>() on province.CountryId equals country.Id
            select new InternProfileWithDetails
            {
                Id = profile.Id,
                UserId = user.Id,
                UserName = user.UserName,
                FullName = (user.Name + " " + user.Surname).Trim(),
                Email = user.Email,
                MentorUserId = mentor.Id,
                MentorFullName = (mentor.Name + " " + mentor.Surname).Trim(),
                WorkplaceId = workplace.Id,
                WorkplaceName = workplace.Name,
                DistrictName = district.Name,
                ProvinceName = province.Name,
                CountryName = country.Name,
                AddressLine = workplace.AddressLine,
                University = profile.University,
                SchoolDepartment = profile.SchoolDepartment,
                StudentNumber = profile.StudentNumber,
                InternshipStartDate = profile.InternshipPeriod.StartDate,
                InternshipEndDate = profile.InternshipPeriod.EndDate,
                RequiredWorkDays = profile.RequiredWorkDays,
                Status = profile.Status
            };
    }
}
