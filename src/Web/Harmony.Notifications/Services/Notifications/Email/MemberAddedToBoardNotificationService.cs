﻿using Hangfire;
using Harmony.Notifications.Persistence;
using Harmony.Domain.Enums;
using Harmony.Notifications.Contracts.Notifications.Email;
using Harmony.Application.Notifications.Email;
using Harmony.Application.Configurations;
using Microsoft.Extensions.Options;
using Grpc.Net.Client;
using Harmony.Api.Protos;

namespace Harmony.Notifications.Services.Notifications.Email
{
    public class MemberAddedToBoardNotificationService : BaseNotificationService, IMemberAddedToBoardNotificationService
    {
        private readonly IEmailService _emailNotificationService;
        private readonly AppEndpointConfiguration _endpointConfiguration;

        public MemberAddedToBoardNotificationService(
            IEmailService emailNotificationService,
            NotificationContext notificationContext,
            IOptions<AppEndpointConfiguration> endpointsConfiguration) : base(notificationContext)
        {
            _emailNotificationService = emailNotificationService;
            _endpointConfiguration = endpointsConfiguration.Value;
        }

        public async Task Notify(MemberAddedToBoardNotification notification)
        {
            await RemovePendingCardJobs(notification.BoardId, EmailNotificationType.MemberAddedToBoard);

            var jobId = BackgroundJob.Enqueue(() => Notify(notification.BoardId, notification.UserId, notification.BoardUrl));

            if (string.IsNullOrEmpty(jobId))
            {
                return;
            }

            _notificationContext.Notifications.Add(new Notification()
            {
                BoardId = notification.BoardId,
                JobId = jobId,
                Type = EmailNotificationType.MemberAddedToBoard,
                DateCreated = DateTime.Now,
            });

            await _notificationContext.SaveChangesAsync();
        }

        public async Task Notify(Guid boardId, string userId, string boardUrl)
        {
            using var channel = GrpcChannel.ForAddress(_endpointConfiguration.HarmonyApiEndpoint);
            var boardServiceClient = new BoardService.BoardServiceClient(channel);

            var boardResponse = await boardServiceClient.GetBoardAsync(new BoardFilterRequest()
            {
                BoardId = boardId.ToString(),
                Workspace = true
            });

            if (!boardResponse.Found)
            {
                return;
            }

            var board= boardResponse.Board;

            var userServiceClient = new UserService.UserServiceClient(channel);
            var userResponse = await userServiceClient.GetUserAsync(
                              new UserFilterRequest
                              {
                                  UserId = userId
                              });

            if (!userResponse.Found)
            {
                return;
            }
            var user = userResponse.User;

            var userNotificationServiceClient = new UserNotificationService.UserNotificationServiceClient(channel);
            var userIsRegisteredResponse = await userNotificationServiceClient.UserIsRegisterForNotificationAsync(
                              new UserIsRegisterForNotificationRequest()
                              {
                                  UserId = userId,
                                  Type = (int)EmailNotificationType.MemberAddedToBoard
                              });

            if (!userIsRegisteredResponse.IsRegistered)
            {
                return;
            }

            var userBoardAccessResponse = await boardServiceClient
                .HasUserAccessToBoardAsync(new UserBoardAccessRequest()
                {
                    UserId= userId,
                    BoardId = boardId.ToString(),
                });

            if (!userBoardAccessResponse.HasAccess)
            {
                return;
            }

            var subject = $"Access {board.Title} in {board.Workspace.Name}";

            var content = $"Dear {user.FirstName} {user.LastName},<br/><br/>" +
                $"You can now access <a href='{boardUrl}' target='_blank'>{board.Title}</a> on {board.Workspace.Name} workspace.";

            await _emailNotificationService.SendEmailAsync(user.Email, subject, content);
        }
    }
}
