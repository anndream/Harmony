﻿using Harmony.Shared.Managers;
using MudBlazor;
using System.Threading.Tasks;

namespace Harmony.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();

        Task<string> GetSelectedWorkspace();

        Task<bool> SetSelectedWorkspace(Guid workspaceId);
    }
}