using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Domain.Shared;
using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public GroupRepository(FinancialTrackerDbContext context)
        {
            _context = context;
        }
        
        public async Task<Result<Guid>> CreateAsync(Group group, GroupMember initialMember)
        {
            var groupEntity = new GroupEntity
            {
                Id = group.Id,
                OwnerId = group.OwnerId,
                Name = group.Name,
                BaseCurrency = group.BaseCurrency,
                TotalLimit = group.TotalLimit,
                CreatedAt = group.CreatedAt
            };

            var memberEntity = new GroupMemberEntity
            {
                Id = initialMember.Id,
                GroupId = group.Id,
                UserId = initialMember.UserId,
                Role = initialMember.Role,
                JoinedAt = initialMember.JoinedAt
            };

            await _context.Groups.AddAsync(groupEntity);
            await _context.GroupMembers.AddAsync(memberEntity);

            await _context.SaveChangesAsync();
            return Result<Guid>.Success(group.Id);
        }

        
        public async Task<Result<Group>> GetByIdAsync(Guid groupId, Guid userId)
        {
            var entity = await _context.Groups
                .AsNoTracking()
                .Include(g => g.Members)
                .Where(g => g.Id == groupId && g.Members.Any(m => m.UserId == userId))
                .FirstOrDefaultAsync();

            if (entity == null)
                return Result<Group>.Failure("Group not found or you are not a member.");

           
            return Result<Group>.Success(MapToDomain(entity));
        }

        
        public async Task<Result<IReadOnlyList<Group>>> GetAllByUserIdAsync(Guid userId)
        {
            var entities = await _context.Groups
                .AsNoTracking()
                .Include(g => g.Members)
                .Where(g => g.Members.Any(m => m.UserId == userId))
                .ToListAsync();

            var result = entities.Select(MapToDomain).ToList();
            return Result<IReadOnlyList<Group>>.Success(result);
        }

        
        public async Task<Result> DeleteGroupAsync(Guid groupId)
        {
            var entity = await _context.Groups.FindAsync(groupId);
            if (entity == null) return Result.Failure("Group not found");
            
            _context.Groups.Remove(entity);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        
        public async Task<Result> RemoveMemberAsync(Guid groupId, Guid userId)
        {
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (member == null) return Result.Failure("Member not found");

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        
        public async Task<Result<Guid>> AddMemberAsync(GroupMember member)
        {
            var entity = new GroupMemberEntity
            {
                Id = member.Id,
                GroupId = member.GroupId,
                UserId = member.UserId,
                Role = member.Role,
                JoinedAt = member.JoinedAt
            };

            await _context.GroupMembers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Result<Guid>.Success(member.Id);
        }

        
        public async Task<bool> ExistsAsync(Guid groupId)
        {
            return await _context.Groups.AnyAsync(g => g.Id == groupId);
        }

        
        private Group MapToDomain(GroupEntity entity)
        {
        
            var members = entity.Members.Select(m =>
                GroupMember.Create(m.Id, m.GroupId, m.UserId, m.Role, m.JoinedAt).Value
            ).ToList();

          
            return Group.Create(
                entity.Id,
                entity.OwnerId,
                entity.Name,
                entity.BaseCurrency,
                entity.TotalLimit,
                entity.CreatedAt,
                members
            ).Value;
        }
        public async Task<Result> UpdateAsync(Group group)
        {
            var entity = await _context.Groups.FirstOrDefaultAsync(g => g.Id == group.Id);

            if (entity == null)
                return Result.Failure("Group not found in DB.");

         
            entity.Name = group.Name;
            entity.TotalLimit = group.TotalLimit;


            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> IsUserInGroup(Guid userId, Guid groupId)
        {
            var userInGroup = await _context.GroupMembers
                .AnyAsync(m => m.UserId == userId && m.GroupId == groupId);

            return userInGroup
                ? Result.Success()
                : Result.Failure("User id not in this group");
        }

        public async Task<bool> IsMemberAsync(Guid groupId, string userEmail)
        {
            return await _context.GroupMembers
                .AsNoTracking()
                .Join(_context.Users,
                      member => member.UserId,
                      user => user.Id,
                      (member, user) => new { member, user })
                .AnyAsync(x => x.member.GroupId == groupId && x.user.Email == userEmail);
        }
    }
}