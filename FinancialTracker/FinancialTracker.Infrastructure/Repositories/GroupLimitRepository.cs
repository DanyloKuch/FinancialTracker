using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class GroupLimitRepository : IGroupLimitRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public GroupLimitRepository(FinancialTrackerDbContext context)
        {
            _context = context;
        }

        private GroupCategoryLimit MapToDomain(GroupCategoryLimitEntity entity) =>
            GroupCategoryLimit.Load(entity.Id, entity.GroupId, entity.CategoryName, entity.LimitAmount);

        public async Task AddAsync(GroupCategoryLimit limit)
        {
            var entity = new GroupCategoryLimitEntity
            {
                Id = limit.Id,
                GroupId = limit.GroupId,
                CategoryName = limit.CategoryName,
                LimitAmount = limit.LimitAmount
            };
            await _context.GroupCategoryLimits.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GroupCategoryLimit limit)
        {
            var entity = await _context.GroupCategoryLimits.FindAsync(limit.Id);
            if (entity != null)
            {
                entity.LimitAmount = limit.LimitAmount;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GroupCategoryLimit?> GetByGroupAndCategoryAsync(Guid groupId, string categoryName)
        {
            var entity = await _context.GroupCategoryLimits
                .FirstOrDefaultAsync(l => l.GroupId == groupId && l.CategoryName == categoryName);
            return entity == null ? null : MapToDomain(entity);
        }

        public async Task<List<GroupCategoryLimit>> GetByGroupIdAsync(Guid groupId)
        {
            var entities = await _context.GroupCategoryLimits
                .AsNoTracking()
                .Where(l => l.GroupId == groupId)
                .ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }
    }
}