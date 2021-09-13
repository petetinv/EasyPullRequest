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
        protected readonly string DeleteBranchPattern = "https://dev.azure.com/{0}/{1}/_apis/git/favorites/refs/{2}?api-version=4.1-preview.1";
        protected readonly string GetBranchesPattern = "https://dev.azure.com/{0}/{1}/_apis/git/repositories/{2}/refs?api-version=4.1";
        protected readonly string GetReposotiriesPattern = "https://{0}.visualstudio.com/{1}/_apis/git/repositories";

        protected readonly string GetPullRequestUrlPattern = "https://{0}.visualstudio.com/{1}/_apis/git/pullrequests?&$skip=0&$top=1000&searchCriteria.status={2}&api-version=5.0";
        private readonly string BasicAuthentication = "Basic ";

        public PullRequestClient(string organization, string projectName, string personalAccessToken)
        {
            GetPullRequestUrlPattern = string.Format(GetPullRequestUrlPattern, organization, projectName, "{0}");
            GetReposotiriesPattern = string.Format(GetReposotiriesPattern, organization, projectName);
            GetBranchesPattern = string.Format(GetBranchesPattern, organization, projectName, "{0}");
            DeleteBranchPattern = string.Format(DeleteBranchPattern, organization, projectName, "{0}");

            BasicAuthentication = string.Concat(BasicAuthentication, ToBase64(Encoding.UTF8, string.Concat(":", personalAccessToken)));
        }

        static protected string ToBase64(Encoding encoding, string value)
        {
            var bytes = encoding.GetBytes(value);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        protected virtual IEnumerable<BaseModel> DeserializeJsonRepo(string json)
        {
            return JObject.Parse(json)["value"].Select(v => new BaseModel
            {
                Id = v.Value<string>("id"),
                Name = v.Value<string>("name"),
                Url = v.Value<string>("url"),
            });
        }
        protected virtual IEnumerable<BaseModel> DeserializeJsonBranch(string json)
        {
            return JObject.Parse(json)["value"].Select(v => new BaseModel
            {
                Id = v.Value<string>("objectId"),
                Name = v.Value<string>("name"),
                Url = v.Value<string>("url"),
            });
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

        public IEnumerable<BaseModel> GetRepositories()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, BasicAuthentication);
                string json = client.DownloadString(GetReposotiriesPattern);
                return DeserializeJsonRepo(json);
            }
        }

        public IEnumerable<BaseModel> GetBranches(string repositoryId)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, BasicAuthentication);
                string url = string.Format(GetBranchesPattern, repositoryId);
                string json = client.DownloadString(url);
                return DeserializeJsonBranch(json);
            }
        }

        public void DeleteBranch(string branchId)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, BasicAuthentication);
                string url = string.Format(DeleteBranchPattern, branchId);
                client.UploadString(url, "DELETE", string.Empty);
            }
        }
    }
}