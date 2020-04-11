using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PullRequetStat
{
    class PullRequestCommentClient
    {
        protected readonly string GetPullRequestThreadsUrlPattern = "https://{0}.visualstudio.com/{1}/_apis/git/repositories/{2}/pullRequests/{3}/threads?api-version=5.0";
        private readonly string BasicAuthentication = "Basic ";

        public PullRequestCommentClient(string organization, string projectName, string personalAccessToken)
        {
            GetPullRequestThreadsUrlPattern = string.Format(GetPullRequestThreadsUrlPattern, organization, projectName, "{0}", "{1}");
            BasicAuthentication = string.Concat(BasicAuthentication, ToBase64(Encoding.UTF8, string.Concat(":", personalAccessToken)));
        }

        static protected string ToBase64(Encoding encoding, string value)
        {
            var bytes = encoding.GetBytes(value);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        protected virtual IEnumerable<PullRequestCommentModel> DeserializeJson(int prId, string json)
        {
            IEnumerable<PullRequestCommentModel> result = JObject.Parse(json)["value"]
                .SelectMany(v => v.SelectTokens("comments").SelectMany(c => c))
                .Where(c => c.Value<string>("commentType") != "system")
                .Select(comment => new PullRequestCommentModel
                {
                    PullRequestId = prId,
                    CreatedBy = comment["author"].Value<string>("uniqueName"),
                    Content = comment.Value<string>("content"),
                    PublishDate = comment.Value<DateTime>("publishedDate"),
                });
            return result ?? Enumerable.Empty<PullRequestCommentModel>();
        }

        public IEnumerable<PullRequestCommentModel> GetComments(IEnumerable<PullRequestModel> prs)
        {
            IEnumerable<PullRequestCommentModel> result = Enumerable.Empty<PullRequestCommentModel>();

            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, BasicAuthentication);
                foreach (var pr in prs)
                {
                    string url = string.Format(GetPullRequestThreadsUrlPattern, pr.RepositoryId, pr.Id);
                    string json = client.DownloadString(url);
                    result = result.Concat(DeserializeJson(pr.Id, json));
                }
            }
            return result;
        }

    }
}