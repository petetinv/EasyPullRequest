using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PullRequetStat
{

    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; set; }

        static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //TODO: use isDevelopment flags
            builder.AddUserSecrets<AzureDevOpsSettings>();

            Configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            Initialize();

            PullRequestClientFactory factory = ServiceProvider.GetService<PullRequestClientFactory>();
            PullRequestClient prClient = factory.GetPRInstance();
            PullRequestCommentClient commentClient = factory.GetCommentInstance();

            var repository = prClient
                .GetRepositories()
                .FirstOrDefault(item => item.Name == "DMU Integration and Services");
            
            if (repository == null)
            {
                throw new Exception($"Reposotory 'DMU Integration and Services' not found!");
            }

            var brancheNames2purge = File.ReadAllLines(@"C:\Users\ng7157C\Desktop\branch-to-purge1.txt")
                .Select(item => new BaseModel { Name = item.Replace("origin/", "refs/heads/") });
            
            var branches2Purge = prClient.GetBranches(repository.Id)
                .Intersect(brancheNames2purge, new BranchNameComparer());


            using (FileStream stream = new FileStream(@"C:\Users\ng7157C\Desktop\branch-to-purge1.output", FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (var branch in branches2Purge)
                {
                    string branchName = branch.Name.Replace("refs/heads/", string.Empty);
                    string commitId = prClient.GetCommitId(repository.Id, branchName);
                    writer.WriteLine($"{commitId};{branchName}");
                    //prClient.DeleteBranch(repository.Id, branch.Name, branch.Id);
                }
            }

            IEnumerable<PullRequestModel> prs = prClient.GetPullRequests(SearchCriterias.Completed)
                .Where(item => item.CreationDate >= new DateTime(2021, 8, 1));

            IEnumerable<PullRequestCommentModel> comments = commentClient.GetComments(prs);

            var path = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            var storage = new PullRequestStorage(prs, comments);
            storage.Save(path);
            Process.Start("explorer", path);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services
                .Configure<AzureDevOpsSettings>(Configuration.GetSection(nameof(AzureDevOpsSettings)))
                .AddSingleton<PullRequestClientFactory>();
        }
    }
}
