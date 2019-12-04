using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace PullRequetStat
{
    class Program
    {
        static void Main(string[] args)
        {
            PullRequestClient client = new PullRequestClient();

            IEnumerable<PullRequestModel> prs = client.GetPullRequests(SearchCriterias.Completed)
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 27));

            var path = "pr.xlsx";
            var storage = new PullRequestStorage(prs);
            storage.Save(path);
            Process.Start(@"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE", path);
        }
    }
}
