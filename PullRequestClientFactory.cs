using Microsoft.Extensions.Options;
using PullRequetStat;

class PullRequestClientFactory
{
    AzureDevOpsSettings settings;
    public PullRequestClientFactory(IOptions<AzureDevOpsSettings> settings)
    {
        this.settings = settings.Value;
    }

    public PullRequestClient GetPRInstance()
    {
        return new PullRequestClient(settings.OrganizationName, settings.ProjectName, settings.Pat);
    }

    public PullRequestCommentClient GetCommentInstance()
    {
        return new PullRequestCommentClient(settings.OrganizationName, settings.ProjectName, settings.Pat);
    }
}
