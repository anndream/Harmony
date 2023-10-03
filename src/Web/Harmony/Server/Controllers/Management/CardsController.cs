﻿using Harmony.Application.Features.Cards.Commands.CreateCard;
using Harmony.Application.Features.Cards.Commands.MoveCard;
using Harmony.Application.Features.Cards.Commands.UpdateCardDescription;
using Harmony.Application.Features.Cards.Commands.UpdateCardStatus;
using Harmony.Application.Features.Cards.Commands.UpdateCardTitle;
using Harmony.Application.Features.Cards.Queries.LoadCard;
using Microsoft.AspNetCore.Mvc;

namespace Harmony.Server.Controllers.Management
{
    public class CardsController : BaseApiController<CardsController>
    {

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await _mediator.Send(new LoadCardQuery(id)));
        }

        /// <summary>
        /// Add a Board
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Products.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(CreateCardCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

		/// <summary>
		/// Add a Board
		/// </summary>
		/// <param name="command"></param>
		/// <returns>Status 200 OK</returns>
		//[Authorize(Policy = Permissions.Products.Create)]
		[HttpPut("{id:guid}/move")]
		public async Task<IActionResult> Put(Guid id, MoveCardCommand command)
		{
			return Ok(await _mediator.Send(command));
		}

        [HttpPut("{id:guid}/description")]
        public async Task<IActionResult> UpdateDescription(Guid id, UpdateCardDescriptionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id:guid}/title")]
        public async Task<IActionResult> UpdateTitle(Guid id, UpdateCardTitleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateCardStatusCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
