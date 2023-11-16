﻿using Harmony.Application.DTO;
using Harmony.Shared.Wrapper;
using MediatR;

namespace Harmony.Application.Features.Labels.Commands.CreateCardLabel;

public class CreateCardLabelCommand : IRequest<IResult<CreateCardLabelResponse>>
{
    public CreateCardLabelCommand(Guid boardId, int? cardId, string color, string title)
    {
        BoardId = boardId;
        CardId = cardId;
        Color = color;
        Title = title;
    }

    public Guid BoardId { get; set; }
    public int? CardId { get; set; }
    public string Color { get; set; }
    public string Title { get; set; }
}
