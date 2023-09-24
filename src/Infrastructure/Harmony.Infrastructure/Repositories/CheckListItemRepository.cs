﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Domain.Entities;
using Harmony.Persistence.DbContext;
using Harmony.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Harmony.Infrastructure.Repositories
{
    public class CheckListItemRepository : ICheckListItemRepository
    {
        private readonly HarmonyContext _context;

        public CheckListItemRepository(HarmonyContext context)
        {
            _context = context;
        }

        public async Task<int> Add(CheckListItem item)
        {
            _context.CheckListItems.Add(item);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<CheckListItem>> GetItems(Guid checklistId)
        {
            return await _context.CheckListItems
                .Where(item => item.CheckListId == checklistId)
                .ToListAsync();
        }
    }
}
