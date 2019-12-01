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

            IEnumerable<double> filtered = prs
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 27))
                .Select(item => item.ClosedDate.Subtract(item.CreationDate).TotalMilliseconds);

            
            double average = filtered.Average(item => item);
            TimeSpan averageTimeSpan = TimeSpan.FromMilliseconds(average);
            Console.WriteLine($"Average: {averageTimeSpan}");
            
            double min = filtered.Min(item => item);
            TimeSpan minTimeSpan = TimeSpan.FromMilliseconds(min);
            Console.WriteLine($"Min: {minTimeSpan}");
            
            double max = filtered.Max(item => item);
            TimeSpan maxTimeSpan = TimeSpan.FromMilliseconds(max);
            Console.WriteLine($"Max: {maxTimeSpan}");
        }
    }
}
