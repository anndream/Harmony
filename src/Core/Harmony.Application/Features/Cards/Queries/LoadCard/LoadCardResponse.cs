﻿using Harmony.Application.DTO;
using Harmony.Domain.Enums;

namespace Harmony.Application.Features.Cards.Queries.LoadCard
{
    /// <summary>
    /// Response for loading card
    /// </summary>
    public class LoadCardResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; } // User created the card
        public BoardListDto BoardList { get; set; }
        public CardStatus Status { get; set; }
        public List<CheckListDto> CheckLists { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public List<CardMemberDto> Members { get; set; }
        public List<LabelDto> Labels { get; set; }
    }
}
