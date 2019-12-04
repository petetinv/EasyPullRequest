using System.Linq;
using System.Collections.Generic;
using System;

namespace PullRequetStat
{
    class Program
    {
        static void Main(string[] args)
        {
            PullRequestClient client = new PullRequestClient();

            IEnumerable<PullRequestModel> prs = client.GetPullRequests(SearchCriterias.Completed)
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 27));

            var storage = new PullRequestStorage(prs);
            storage.Save("pr.xlsx");
        }
    }
}
