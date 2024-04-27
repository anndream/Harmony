using Harmony.Application.Features.SourceControl.Commands.CreateBranch;
using Harmony.Application.Features.SourceControl.Commands.DeleteBranch;
using Harmony.Application.SourceControl.Features.SourceControl.Commands.CreatePullRequest;
using Harmony.Application.SourceControl.Features.SourceControl.Commands.CreatePush;
using Harmony.Application.SourceControl.Features.SourceControl.Queries.GetCardBranches;
using Harmony.Domain.Enums.SourceControl;
using Harmony.Domain.SourceControl;
using Harmony.Integrations.SourceControl.Constants;
using Harmony.Integrations.SourceControl.WebhookRequests.GitHub;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Harmony.Integrations.SourceControl.Controllers
{
    public class GithubController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GithubController> _logger;
        private static char[] ALLOWED_CHARS = [' ','_','-','/'];
        public GithubController(
            IMediator mediator,
            ILogger<GithubController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Github webhook
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var eventType = Request.Headers[GithubConstants.GitHubEventHeader];

            if (eventType == "ping")
            {
                return Ok();
            }

            string postData = null;

            // Read the post data from the request body
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                postData = await reader.ReadToEndAsync();
            }

            switch (eventType)
            {
                case "push":
                    await HandlePushWebhook(postData);
                    break;
                case "create":
                case "delete":
                    await HandleEventWebhook(postData, eventType);
                    break;
                case "pull_request":
                    await HandlePullRequestWebhook(postData);
                    break;
                default:
                    break;
            }

            return Ok();
        }

        private async Task HandleEventWebhook(string postData, string eventType)
        {
            var request = JsonSerializer.Deserialize<GitHubBranchRequest>(postData);

            if (request.Type.Equals("branch"))
            {
                switch (eventType)
                {
                    case "create":
                        await _mediator.Send(new CreateBranchCommand()
                        {
                            SerialKey = GetSerialKey(request.Ref),
                            Name = request.Ref,
                            Repository = new Repository()
                            {
                                RepositoryId = request.Repository.Id.ToString(),
                                Name = request.Repository.Name,
                                FullName = request.Repository.FullName,
                                Url = request.Repository.HtmlUrl,
                                Provider = SourceControlProvider.GitHub
                            },
                            Creator = new RepositoryUser(request.Sender.Login, request.Sender.AvatarUrl, request.Sender.HtmlUrl)
                        });
                        break;
                    case "delete":
                        await _mediator.Send(new DeleteBranchCommand()
                        {
                            Name = request.Ref,
                            RepositoryId = request.Repository.Id.ToString()
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task HandlePushWebhook(string postData)
        {
            var request = JsonSerializer.Deserialize<GitHubPushRequest>(postData);

            var push = new CreatePushCommand()
            {
                Branch = request.Ref.Split('/').Last(),
                Repository = new Repository()
                {
                    RepositoryId = request.Repository.Id.ToString(),
                    Name = request.Repository.Name,
                    FullName = request.Repository.FullName,
                    Url = request.Repository.HtmlUrl,
                    Provider = SourceControlProvider.GitHub
                },
                Commits = request.Commits.Select(commit =>
                    new Commit()
                    {
                        Url = commit.Url,
                        Author = new Author()
                        {
                            Email = commit.Author.Email,
                            Name = commit.Author.Name,
                            Username = commit.Author.Username,
                        },
                        Id = commit.Id,
                        Message = commit.Message,
                        Timestamp = commit.Timestamp,
                        Added = commit.Added ?? new List<string>(),
                        Removed = commit.Removed ?? new List<string>(),
                        Modified = commit.Modified ?? new List<string>(),
                    }).ToList(),
                Sender = new RepositoryUser(request.Sender.Login, request.Sender.AvatarUrl, request.Sender.HtmlUrl)
            };

            await _mediator.Send(push);
        }

        private async Task HandlePullRequestWebhook(string postData)
        {
            var request = JsonSerializer.Deserialize<GitHubPullRequest>(postData);

            if (request != null)
            {
                var pullRequest = new CreatePullRequestCommand()
                {
                    Id = request.PullRequest.Id.ToString(),
                    Action = request.Action,
                    HtmlUrl = request.PullRequest.HtmlUrl,
                    DiffUrl = request.PullRequest.DiffUrl,
                    State = request.PullRequest.State
                    switch
                    {
                        "opened" => PullRequestState.Opened,
                        "closed" => request.PullRequest.MergedAt.HasValue ? PullRequestState.Merged : PullRequestState.Closed,
                        _ => PullRequestState.Opened
                    },
                    Title = request.PullRequest.Title,
                    Number = request.PullRequest.Number,
                    CreatedAt = request.PullRequest.CreatedAt,
                    UpdatedAt = request.PullRequest.UpdatedAt,
                    ClosedAt = request.PullRequest.ClosedAt,
                    MergedAt = request.PullRequest.MergedAt,
                    SourceBranch = request.PullRequest.Head.Ref,
                    TargetBranch = request.PullRequest.Base.Ref,
                    MergeCommitSha = request.PullRequest.MergeCommitSha,
                    Assignees = request.PullRequest.Assignees.Select(a =>
                        new RepositoryUser(a.Login, a.AvatarUrl, a.HtmlUrl)).ToList(),
                    Reviewers = request.PullRequest.RequestedReviewers.Select(a =>
                        new RepositoryUser(a.Login, a.AvatarUrl, a.HtmlUrl)).ToList(),
                    Repository = new Repository()
                    {
                        RepositoryId = request.Repository.Id.ToString(),
                        Name = request.Repository.Name,
                        FullName = request.Repository.FullName,
                        Url = request.Repository.HtmlUrl,
                        Provider = SourceControlProvider.GitHub
                    },
                    Sender = new RepositoryUser(request.Sender.Login, request.Sender.AvatarUrl, request.Sender.HtmlUrl)
                };

                await _mediator.Send(pullRequest);
            }
        }

        private string GetSerialKey(string text)
        {
            List<string> matches = Regex.Matches(text, @"([a-zA-Z]{3,5})-(\d+)", RegexOptions.IgnoreCase)
                       .Cast<Match>()
                       .Select(x => x.Value).ToList();

            foreach(var match in matches)
            {
                var matchIndex = text.IndexOf(match, StringComparison.InvariantCultureIgnoreCase);

                if(matchIndex == 0 && NextCharAllowed(text, match, matchIndex))
                {
                    return match;
                }
                else if(matchIndex > 0)
                {
                    var previousChar = text[matchIndex - 1];
                    if (PreviewCharAllowed(text, matchIndex) && NextCharAllowed(text, match, matchIndex))
                    {
                        return match;
                    }
                }
            }

            return null;
        }

        private bool PreviewCharAllowed(string original, int termIndex)
        {
            var previousChar = original[termIndex - 1];

            if (ALLOWED_CHARS.Contains(previousChar))
            {
                return true;
            }

            return false;
        }

        private bool NextCharAllowed(string original, string term, int termIndex)
        {
            if(original.Length == term.Length + termIndex)
            {
                return true;
            }

            var nextChar = original[termIndex + term.Length];

            if (ALLOWED_CHARS.Contains(nextChar))
            {
                return true;
            }

            return false;
        }
    }
}
