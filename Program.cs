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
            PullRequestClient client = factory.GetInstance();

            IEnumerable<PullRequestModel> prs = client.GetPullRequests(SearchCriterias.Completed)
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 27));

            var path = "pr.xlsx";
            var storage = new PullRequestStorage(prs);
            storage.Save(path);
            Process.Start(@"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE", path);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services
                .Configure<AzureDevOpsSettings>(Configuration.GetSection(nameof(AzureDevOpsSettings)))
                .AddSingleton<PullRequestClientFactory>();
        }
    }
}
