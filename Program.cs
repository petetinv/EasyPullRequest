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

            IEnumerable<Pullrequest> prs = client.GetPullRequests(SearchCriterias.Completed);

            double averageTotalMilliseconds = prs
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 25))
                .Average(item => item.ClosedDate.Subtract(item.CreationDate).TotalMilliseconds);
            
            TimeSpan averageTimeSpan = TimeSpan.FromMilliseconds(averageTotalMilliseconds);

            System.Console.WriteLine(averageTimeSpan);
        }
    }
}
