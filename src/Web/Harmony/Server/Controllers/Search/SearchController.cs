﻿using Harmony.Application.Features.Search.Queries.GlobalSearch;
using Microsoft.AspNetCore.Mvc;

namespace Harmony.Server.Controllers.Search
{
    /// <summary>
    /// Controller for searching
    /// </summary>
    public class SearchController : BaseApiController<SearchController>
    {
        [HttpGet]
        public async Task<IActionResult> Get(Guid boardId, string term)
        {
            return Ok(await _mediator.Send(new GlobalSearchQuery(boardId, term)));
        }
    }
}
