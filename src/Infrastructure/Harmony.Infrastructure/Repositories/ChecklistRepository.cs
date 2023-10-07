﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Domain.Entities;
using Harmony.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Harmony.Infrastructure.Repositories
{
    public class ChecklistRepository : IChecklistRepository
    {
        private readonly HarmonyContext _context;

        public ChecklistRepository(HarmonyContext context)
        {
            _context = context;
        }

		public async Task<CheckList?> Get(Guid ChecklistId)
		{
			return await _context.CheckLists.FirstOrDefaultAsync(Checklist => Checklist.Id == ChecklistId);
		}

		public async Task<int> Add(CheckList checklist)
		{
			_context.CheckLists.Add(checklist);

			return await _context.SaveChangesAsync();
		}

		public async Task<int> Update(CheckList checklist)
		{
			_context.CheckLists.Update(checklist);

			return await _context.SaveChangesAsync();
		}

        public async Task<List<CheckList>> GetCardCheckLists(Guid cardId)
        {
            return await _context.CheckLists
				.Where(checklist => checklist.CardId == cardId)
				.ToListAsync();
        }

        public async Task<int> CountCardCheckLists(Guid cardId)
        {
            return await _context.CheckLists
                .Where(checklist => checklist.CardId == cardId)
                .CountAsync();
        }
    }
}
