using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class PullRequestClient
{
    const string GetPullRequestUrlPattern = "https://airbus-caddmu.visualstudio.com/eBAM/_apis/git/pullrequests?&$skip=0&searchCriteria.status={0}&api-version=5.0";

    protected virtual IEnumerable<Pullrequest> DeserializeJson(string json)
    {
        JObject jobject = JObject.Parse(json);
        foreach (JToken token in jobject["value"])
        {
            yield return new Pullrequest 
            { 
                ClosedDate = token["closedDate"].Value<DateTime>(),
                CreationDate = token["creationDate"].Value<DateTime>()
            };
        }

        //return JsonConvert.DeserializeObject<IEnumerable<Pullrequest>>(json);
    }

    public IEnumerable<Pullrequest> GetPullRequests(string searchCriteria)
    {
        using (WebClient client = new WebClient())
        {
            client.Headers.Add(HttpRequestHeader.Authorization, "Basic OmZsNnhxcGJ6ZWx3amdndGdzZnVhbXh1aWhjaWRrd25uam5sc3JyeXJkMnI3N2ZoZXIzNWE=");
            string url = string.Format(GetPullRequestUrlPattern, searchCriteria);
            return DeserializeJson(client.DownloadString(url));
        }
    }

}

class Pullrequest
{
    public DateTime CreationDate { get; set; }
    public DateTime ClosedDate { get; set; }
}

class SearchCriterias
{
    static public string Completed = "completed";
}