using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

class PullRequestClient
{
    const string GetPullRequestUrlPattern = "https://airbus-caddmu.visualstudio.com/eBAM/_apis/git/pullrequests?&$skip=0&searchCriteria.status={0}&api-version=5.0";

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
            client.Headers.Add(HttpRequestHeader.Authorization, "Basic OmZsNnhxcGJ6ZWx3amdndGdzZnVhbXh1aWhjaWRrd25uam5sc3JyeXJkMnI3N2ZoZXIzNWE=");
            string url = string.Format(GetPullRequestUrlPattern, searchCriteria);
            string json = client.DownloadString(url);
            //File.WriteAllText("toto.json", client.DownloadString(url));
            return DeserializeJson(json);
        }
    }

}
