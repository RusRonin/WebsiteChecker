using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteAvailabilityTracker
{
    class Program
    {
        private ISiteDatabaseProvider databaseProvider;

        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureService(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var userInterface = serviceProvider.GetService<ICommandUserInterface>();

            var sites = serviceProvider.GetService<ISiteList>();

            /*
            TimerCallback timerCallback = new TimerCallback(siteChecker.CheckSitesAsync);
            Timer timer = new Timer(timerCallback, sites, 1000, 500);*/

            userInterface.PrintCommandList();
            bool endWork = false;
            while (!endWork)
            {
                userInterface.ReadCommand(sites, ref endWork);
            }
        }

        private static void ConfigureService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISiteDatabaseProvider, TXTSiteDatabaseProvider>();
            serviceCollection.AddScoped<ISiteChecker, SiteChecker>();
            serviceCollection.AddScoped<ICommandUserInterface, UserInterface>();
            serviceCollection.AddScoped<ISiteList, SiteList>();
        }
    }
}
