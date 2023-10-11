﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Domain.Entities;
using Harmony.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Harmony.Infrastructure.Repositories
{
    public class CardActivityRepository : ICardActivityRepository
    {
        private readonly HarmonyContext _context;

        public CardActivityRepository(HarmonyContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CardActivity activity)
        {
            await _context.CardActivities.AddAsync(activity);

            return await _context.SaveChangesAsync();
        }
    }
}
