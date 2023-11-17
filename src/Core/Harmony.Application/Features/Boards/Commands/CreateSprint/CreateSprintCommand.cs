﻿using Harmony.Application.DTO;
using Harmony.Domain.Enums;
using Harmony.Shared.Wrapper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Application.Features.Boards.Commands.CreateSprint
{
    /// <summary>
    /// Command to create sprint
    /// </summary>
    public class CreateSprintCommand : IRequest<Result<SprintDto>>
    {
        [Required]
        public Guid BoardId { get; set; }

        public CreateSprintCommand(Guid boardId)
        {
            BoardId = boardId;
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Goal { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }
    }
}
