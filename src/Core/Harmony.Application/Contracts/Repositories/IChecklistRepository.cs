﻿using Harmony.Domain.Entities;

namespace Harmony.Application.Contracts.Repositories
{
    public interface ICheckListRepository
    {
        Task<int> Add(CheckList card);
        Task<CheckList?> Get(Guid checklistId);
        Task<List<CheckList>> GetCardCheckLists(Guid cardId);
        Task<int> Update(CheckList checklist);
        Task<int> CountCardCheckLists(Guid cardId);
    }
}
