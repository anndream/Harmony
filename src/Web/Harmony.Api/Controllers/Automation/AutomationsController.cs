﻿using AutoMapper;
using Grpc.Net.Client;
using Harmony.Api.Controllers.Management;
using Harmony.Application.Configurations;
using Harmony.Application.DTO.Automation;
using Harmony.Application.Features.Automations.Commands.CreateAutomation;
using Harmony.Application.Features.Automations.Commands.ToggleAutomation;
using Harmony.Automations.Protos;
using Harmony.Domain.Enums;
using Harmony.Shared.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Harmony.Api.Controllers.Automation
{
    public class AutomationsController : BaseApiController<CardsController>
    {
        private readonly AppEndpointConfiguration _endpointConfiguration;
        private readonly IMapper _mapper;

        public AutomationsController(IOptions<AppEndpointConfiguration> endpointConfiguration,
            IMapper mapper)
        {
            _endpointConfiguration = endpointConfiguration.Value;
            _mapper = mapper;
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            using var channel = GrpcChannel.ForAddress(_endpointConfiguration.AutomationEndpoint);
            var client = new AutomationService.AutomationServiceClient(channel);

            var getAutomationTemplatesResult = await client
                .GetAutomationTemplatesAsync(new GetAutomationTemplatesRequest());;

            return Ok(getAutomationTemplatesResult.Success ?
             Result<List<AutomationTemplateDto>>.Success(_mapper
             .Map<List<AutomationTemplateDto>>(getAutomationTemplatesResult.Templates),
                getAutomationTemplatesResult?.Messages?.FirstOrDefault())
             : Result.Fail(getAutomationTemplatesResult.Messages.ToList()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAutomationCommand command)
        {
            using var channel = GrpcChannel.ForAddress(_endpointConfiguration.AutomationEndpoint);
            var client = new AutomationService.AutomationServiceClient(channel);

            var createAutomationResult = await client.CreateAutomationAsync(
                              new CreateAutomationRequest()
                              {
                                  Automation = command.Automation,
                                  Type = (int)command.Type
                              });

            return Ok(createAutomationResult.Success ?
             Result<string>.Success(createAutomationResult.AutomationId, 
                createAutomationResult?.Messages?.FirstOrDefault())
             : Result.Fail(createAutomationResult.Messages.ToList()));
        }

        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> Update(string id, ToggleAutomationCommand command)
        {
            using var channel = GrpcChannel.ForAddress(_endpointConfiguration.AutomationEndpoint);
            var client = new AutomationService.AutomationServiceClient(channel);

            var toggleAutomationResult = await client.ToggleAutomationAsync(
                              new ToggleAutomationRequest()
                              {
                                  AutomationId = command.AutomationId,
                                  Enabled = command.Enabled
                              });

            return Ok(toggleAutomationResult.Success ?
             Result<bool>.Success(true, toggleAutomationResult?.Messages?.FirstOrDefault())
             : Result.Fail(toggleAutomationResult.Messages.ToList()));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            using var channel = GrpcChannel.ForAddress(_endpointConfiguration.AutomationEndpoint);
            var client = new AutomationService.AutomationServiceClient(channel);

            var removeAutomationResult = await client.RemoveAutomationAsync(
                              new RemoveAutomationRequest()
                              {
                                  AutomationId = id
                              });

            return Ok(removeAutomationResult.Success ?
             Result<bool>.Success(true, removeAutomationResult?.Messages?.FirstOrDefault())
             : Result.Fail(removeAutomationResult.Messages.ToList()));
        }

        [HttpGet("{boardId:guid}/types/{type:int}")]
        public async Task<IActionResult> GetAutomations(Guid boardId, AutomationType type)
        {
            using var channel = GrpcChannel.ForAddress(_endpointConfiguration.AutomationEndpoint);
            var client = new AutomationService.AutomationServiceClient(channel);

            var getAutomationsResult = await client
                .GetAutomationsAsync(new GetAutomationsRequest()
                {
                    AutomationType = (int)type,
                    BoardId = boardId.ToString()
                });


            var list = getAutomationsResult.Automations.Select(auto =>
            {
                switch (type)
                {
                    case AutomationType.SyncParentAndChildIssues:
                        return JsonSerializer.Deserialize<SyncParentAndChildIssuesAutomationDto>(auto);
                    default:
                        return null;
                }
            }).ToList();

            return Ok(Result<List<SyncParentAndChildIssuesAutomationDto>>.Success(list));
        }
    }
}
