using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteAvailabilityTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureService(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var userInterface = serviceProvider.GetService<ICommandUserInterface>();

            var sites = serviceProvider.GetService<ISiteList>();

            var siteChecker = serviceProvider.GetService<ISiteChecker>();

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            siteChecker.CheckSitesAsync((ISiteList)sites.Clone(), token);

            userInterface.PrintCommandList();
            bool endWork = false;
            bool asyncCheckRestart = false;
            while (!endWork)
            {
                userInterface.ReadCommand(sites, cts, ref endWork, ref asyncCheckRestart);
                if (asyncCheckRestart)
                {
                    cts = new CancellationTokenSource();
                    token = cts.Token;

                    siteChecker.CheckSitesAsync((ISiteList)sites.Clone(), token);
                }
            }
        }

        private static void ConfigureService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISiteDatabaseProvider, TXTSiteDatabaseProvider>();
            serviceCollection.AddScoped<ISiteChecker, SiteChecker>();
            serviceCollection.AddScoped<ICommandUserInterface, UserInterface>();
            serviceCollection.AddScoped<ISiteList, SiteList>();
            serviceCollection.AddScoped<IScanResultFileProvider, ScanResultFileProvider>();
        }
    }
}
