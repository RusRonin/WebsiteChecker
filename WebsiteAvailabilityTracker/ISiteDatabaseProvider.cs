using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public interface ISiteDatabaseProvider
    {
        List<Site> LoadSites();
        void SaveSites(ISiteList sites);
    }
}
