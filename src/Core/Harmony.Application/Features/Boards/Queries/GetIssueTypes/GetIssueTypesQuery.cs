﻿using Harmony.Application.DTO;
using Harmony.Application.Requests;
using Harmony.Shared.Wrapper;
using MediatR;

namespace Harmony.Application.Features.Workspaces.Queries.GetIssueTypes
{
    public class GetIssueTypesQuery : IRequest<IResult<List<IssueTypeDto>>>
    {
        public Guid BoardId { get; set; }

        public GetIssueTypesQuery(Guid boardId)
        {
            BoardId = boardId;
        }
    }
}