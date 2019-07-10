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
            SiteListChanger.ConfigureXml();
            List<Site> sites = SiteListChanger.LoadSites();

            SiteChecker siteChecker = SiteChecker.GetSiteChecker();

            TimerCallback timerCallback = new TimerCallback(siteChecker.CheckSitesAsync);
            Timer timer = new Timer(timerCallback, sites, 1000, 500);

            UserInterface.PrintCommandList();
            bool endWork = false;
            while (!endWork)
            {
                UserInterface.ReadCommand(ref sites, ref endWork);
            }
        }


    }
}
