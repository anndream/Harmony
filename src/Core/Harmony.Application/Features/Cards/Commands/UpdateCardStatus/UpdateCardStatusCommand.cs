﻿using Harmony.Application.DTO;
using Harmony.Domain.Enums;
using Harmony.Shared.Wrapper;
using MediatR;

namespace Harmony.Application.Features.Cards.Commands.UpdateCardStatus;

public class UpdateCardStatusCommand : IRequest<Result<bool>>
{
    public Guid CardId { get; set; }
    public CardStatus Status { get; set; }

	public UpdateCardStatusCommand(Guid cardId, CardStatus status)
	{
		CardId = cardId;
        Status = status;
	}
}
