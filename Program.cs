﻿using System.Linq;
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

            IEnumerable<PullRequestModel> prs = prClient.GetPullRequests(SearchCriterias.Completed)
                .Where(item => item.CreationDate >= new DateTime(2019, 11, 27));

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
