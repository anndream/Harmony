﻿using Harmony.Application.Contracts.Repositories;
using Harmony.Domain.Entities;
using Harmony.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Harmony.Infrastructure.Repositories
{
    public class UserWorkspaceRepository : IUserWorkspaceRepository
    {
        private readonly HarmonyContext _context;

        public UserWorkspaceRepository(HarmonyContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetWorkspaceUsers(Guid workspaceId)
        {
            return await _context.UserWorkspaces
                .Where(userWorkspace => userWorkspace.WorkspaceId == workspaceId)
                .Select(userWorkspace => userWorkspace.UserId)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(UserWorkspace userWorkspaceRepository)
        {
            await _context.UserWorkspaces.AddAsync(userWorkspaceRepository);

            return await _context.SaveChangesAsync();
        }
    }
}
