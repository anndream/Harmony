﻿namespace Harmony.Client.Infrastructure.Routes
{
    public static class UserNotificationsEndpoints
    {
        public static string Index = "api/usernotifications";

        public static string User(string userId) => $"{Index}/{userId}/";
    }
}