using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteAvailabilityTracker
{
    internal class SiteChecker
    {
        //singleton

        const int checkingTimeStep = 500;
        const int maxCheckingInterval = 10000;

        private int millisecondsPassed;


        private static SiteChecker siteChecker = null;

        private SiteChecker()
        {
            millisecondsPassed = checkingTimeStep;
        }

        internal static SiteChecker GetSiteChecker()
        {
            if (siteChecker == null)
            {
                siteChecker = new SiteChecker();
            }
            return siteChecker;
        }

        internal static int GetCheckingTimeStep()
        {
            return checkingTimeStep;
        }

        internal static int GetMaxCheckingInterval()
        {
            return maxCheckingInterval;
        }

        internal void CheckSites(List<Site> sites)
        {
            foreach (Site site in sites)
            {
                if (millisecondsPassed % site.CheckingFrequency == 0)
                {
                    //временная заглушка вместо проверки сайтов
                    UserInterface.PrintString(site.Address, true);
                }
                if (millisecondsPassed == maxCheckingInterval)
                {
                    millisecondsPassed = checkingTimeStep;
                }
                else
                {
                    millisecondsPassed += checkingTimeStep;
                }
            }
        }

        internal async void CheckSitesAsync(object obj)
        {
            List<Site> sites = obj as List<Site>;
            if (sites as List<Site> != null)
            {
                await Task.Run(() => CheckSites(sites));
            }
        }
    }
}
