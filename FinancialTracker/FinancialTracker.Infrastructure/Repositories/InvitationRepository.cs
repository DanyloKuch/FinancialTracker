using FinancialTracker.Domain.Enums;
using FinancialTracker.Domain.Interfaces;
using FinancialTracker.Domain.Models;
using FinancialTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly FinancialTrackerDbContext _context;

        public InvitationRepository(FinancialTrackerDbContext context)
        {
            _context = context;
        }

        private Invitation MapToDomain(InvitationEntity entity)
        {
            return Invitation.Load(entity.Id, entity.GroupId, entity.InviterId, entity.InviteeEmail, entity.Status, entity.CreatedAt);
        }

        public async Task AddAsync(Invitation invitation)
        {
            var entity = new InvitationEntity
            {
                Id = invitation.Id,
                GroupId = invitation.GroupId,
                InviterId = invitation.InviterId,
                InviteeEmail = invitation.InviteeEmail,
                Status = invitation.Status,
                CreatedAt = invitation.CreatedAt
            };
            await _context.Invitations.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Invitation?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Invitations.FindAsync(id);
            return entity == null ? null : MapToDomain(entity);
        }

        public async Task<List<Invitation>> GetPendingByEmailAsync(string email)
        {
            var entities = await _context.Invitations
                .AsNoTracking()
                .Where(i => i.InviteeEmail == email && i.Status == InvitationStatus.Pending)
                .ToListAsync();
            return entities.Select(MapToDomain).ToList();
        }

        public async Task UpdateAsync(Invitation invitation)
        {
            var entity = await _context.Invitations.FindAsync(invitation.Id);
            if (entity != null)
            {
                entity.Status = invitation.Status;
                await _context.SaveChangesAsync();
            }
        }
    }
}