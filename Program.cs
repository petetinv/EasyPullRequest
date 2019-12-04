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

            IEnumerable<PullRequestModel> prs = client.GetPullRequests(SearchCriterias.Completed);

            IEnumerable<TimeSpan> filtered = prs
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 27))
                .Select(item => item.Duration);

            double average = filtered.Average(item => item.TotalMilliseconds);
            Console.WriteLine($"Average: {TimeSpan.FromMilliseconds(average)}");
            
            double min = filtered.Min(item => item.TotalMilliseconds);
            Console.WriteLine($"Min: {TimeSpan.FromMilliseconds(min)}");
            
            double max = filtered.Max(item => item.TotalMilliseconds);
            Console.WriteLine($"Max: {TimeSpan.FromMilliseconds(max)}");

            
        }
    }
}
