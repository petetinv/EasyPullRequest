using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PullRequetStat
{
    class PullRequestClient
    {
        protected readonly string GetPullRequestUrlPattern = "https://{0}.visualstudio.com/{1}/_apis/git/pullrequests?&$skip=0&$top=1000&searchCriteria.status={2}&api-version=5.0";
        private readonly string BasicAuthentication = "Basic ";

        public PullRequestClient(string organization, string projectName, string personalAccessToken)
        {
            GetPullRequestUrlPattern = string.Format(GetPullRequestUrlPattern, organization, projectName, "{0}");
            BasicAuthentication = string.Concat(BasicAuthentication, ToBase64(Encoding.UTF8, string.Concat(":", personalAccessToken)));
        }

        static protected string ToBase64(Encoding encoding, string value)
        {
            var bytes = encoding.GetBytes(value);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        protected virtual IEnumerable<PullRequestModel> DeserializeJson(string json)
        {
            return JObject.Parse(json)["value"].Select(v => new PullRequestModel
            {
                Id = v.Value<int>("pullRequestId"),
                Title = v.Value<string>("title"),
                Description = v.Value<string>("description"),
                RepositoryId = v["repository"].Value<string>("id"),
                RepositoryName = v["repository"].Value<string>("name"),
                CreationDate = v.Value<DateTime>("creationDate"),
                ClosedDate = v.Value<DateTime>("closedDate"),
                CreatedBy = v["createdBy"].Value<string>("uniqueName"),
                Reviewers = v.SelectTokens("reviewers").SelectMany(r => r).Values<string>("uniqueName")
            });
        }

        public IEnumerable<PullRequestModel> GetPullRequests(string searchCriteria)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, BasicAuthentication);
                string url = string.Format(GetPullRequestUrlPattern, searchCriteria);
                string json = client.DownloadString(url);
                return DeserializeJson(json);
            }
        }

    }
}