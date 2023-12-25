﻿using Harmony.Application.DTO;
using static Harmony.Application.Events.BoardListArchivedEvent;

namespace Harmony.Application.Contracts.Services.Hubs
{
    /// <summary>
    /// Service for Hub client notifications
    /// </summary>
    public interface IHubClientNotifierService
    {
        Task AddBoardList(Guid boardId, BoardListDto boardList);
        Task UpdateBoardListTitle(Guid boardId, Guid boardListId, string title);
        Task UpdateCardTitle(Guid boardId, Guid cardId, string title);
        Task UpdateCardDescription(Guid boardId, Guid cardId, string description);
        Task UpdateCardStoryPoints(Guid boardId, Guid cardId, short? storyPoints);
        Task UpdateCardDates(Guid boardId, Guid cardId, DateTime? startDate, DateTime? dueDate);
        Task ToggleCardLabel(Guid boardId, Guid cardId, LabelDto label);
        Task AddCardAttachment(Guid boardId, Guid cardId, AttachmentDto attachment);
        Task RemoveCardAttachment(Guid boardId, Guid cardId, Guid attachmentId);
        Task CreateCheckListItem(Guid boardId, Guid cardId);
        Task ToggleCardListItemChecked(Guid boardId, Guid cardId, Guid listItemId, bool isChecked);
        Task ArchiveBoardList(Guid boardId, Guid archivedList, List<BoardListOrder> positions);
        Task RemoveCardLabel(Guid boardId, Guid cardLabelId);
        Task UpdateBoardListsPositions(Guid boardId, Dictionary<Guid, short> positions);
        Task AddCardMember(Guid boardId, Guid cardId, CardMemberDto cardMember);
        Task RemoveCardMember(Guid boardId, Guid cardId, CardMemberDto cardMember);
        Task RemoveCheckList(Guid boardId, Guid checkListId, Guid cardId, int totalItems, int totalItemsCompleted);
        Task UpdateCardPosition(Guid boardId, Guid cardId, Guid previousBoardListId, Guid nextBoardListId,
            short previousPosition, short newPosition, Guid updateId);
        Task UpdateCardIssueType(Guid boardId, Guid cardId, IssueTypeDto issueType);
    }
}
