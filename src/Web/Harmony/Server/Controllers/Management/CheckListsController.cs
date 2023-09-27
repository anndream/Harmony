﻿using Harmony.Application.Features.Boards.Commands.CreateList;
using Harmony.Application.Features.Cards.Commands.CreateChecklist;
using Harmony.Application.Features.Cards.Commands.CreateCheckListItem;
using Harmony.Application.Features.Cards.Commands.UpdateCardTitle;
using Harmony.Application.Features.Lists.Commands.ArchiveList;
using Harmony.Application.Features.Lists.Commands.UpdateListTitle;
using Microsoft.AspNetCore.Mvc;

namespace Harmony.Server.Controllers.Management
{
    public class CheckListsController : BaseApiController<CheckListsController>
    {

		[HttpPost]
		public async Task<IActionResult> Post(CreateCheckListCommand command)
		{
			return Ok(await _mediator.Send(command));
		}

        [HttpPost("{id:guid}/items")]
        public async Task<IActionResult> AddItem(CreateCheckListItemCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id:guid}/title")]
        public async Task<IActionResult> UpdateTitle(Guid id, UpdateListTitleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
