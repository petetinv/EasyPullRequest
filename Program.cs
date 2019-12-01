using System;
using System.Collections.Generic;

namespace PullRequetStat
{
    class Program
    {
        static void Main(string[] args)
        {
            PullRequestClient client = new PullRequestClient();

            IEnumerable<Pullrequest> prs = client.GetPullRequests(SearchCriterias.Completed);
        }
    }
}
