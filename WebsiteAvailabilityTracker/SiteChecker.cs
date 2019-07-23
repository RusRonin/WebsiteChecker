using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteAvailabilityTracker
{
    public class SiteChecker : ISiteChecker
    { 

        const int checkingTimeStep = 500;
        const int maxCheckingInterval = 10000;

        private int millisecondsPassed;

        internal SiteChecker()
        {
            millisecondsPassed = checkingTimeStep;
        }

        internal static int GetCheckingTimeStep()
        {
            return checkingTimeStep;
        }

        internal static int GetMaxCheckingInterval()
        {
            return maxCheckingInterval;
        }

        internal void CheckSites(ISiteList sites)
        {
            foreach (Site site in sites)
            {
                if (millisecondsPassed % site.CheckingFrequency == 0)
                {
                    //временная заглушка вместо проверки сайтов
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

        public void CheckSite(Site site)
        {
            //checking site
        }

        internal async void CheckSitesAsync(object obj)
        {
            ISiteList sites = obj as ISiteList;
            if (sites as List<Site> != null)
            {
                await Task.Run(() => CheckSites(sites));
            }
        }
    }
}
