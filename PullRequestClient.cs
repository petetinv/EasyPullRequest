using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

class PullRequestClient
{
    protected readonly string GetPullRequestUrlPattern = "https://{0}.visualstudio.com/{1}/_apis/git/pullrequests?&$skip=0&searchCriteria.status={2}&api-version=5.0";
    private readonly string BasicAuthentication = "Basic ";

    public PullRequestClient(string organization, string projectName, string personalAccessToken)
    {
        GetPullRequestUrlPattern = string.Format(GetPullRequestUrlPattern, organization, projectName, "{0}");
        this.BasicAuthentication = string.Concat(this.BasicAuthentication, ToBase64(Encoding.UTF8, personalAccessToken));
    }

    static protected string ToBase64(Encoding encoding, string value)
    {
        var bytes = encoding.GetBytes(value);
        string base64 = Convert.ToBase64String(bytes);
        return base64;
    }

    protected virtual IEnumerable<PullRequestModel> DeserializeJson(string json)
    {
        JObject jobject = JObject.Parse(json);
        foreach (JToken token in jobject["value"])
        {
            yield return new PullRequestModel 
            { 
                ClosedDate = token["closedDate"].Value<DateTime>(),
                CreationDate = token["creationDate"].Value<DateTime>()
            };
        }
    }

    public IEnumerable<PullRequestModel> GetPullRequests(string searchCriteria)
    {
        using (WebClient client = new WebClient())
        {
            client.Headers.Add(HttpRequestHeader.Authorization, this.BasicAuthentication);
            string url = string.Format(GetPullRequestUrlPattern, searchCriteria);
            string json = client.DownloadString(url);
            //File.WriteAllText("toto.json", client.DownloadString(url));
            return DeserializeJson(json);
        }
    }

}
