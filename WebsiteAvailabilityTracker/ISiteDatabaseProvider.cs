using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public interface ISiteDatabaseProvider : IEnumerable
    {
        List<Site> GetClonedSiteList();
        void SaveSites(List<Site> sites);
        void AddSite(Site site);
        void RemoveSite(Site site);
        void ReloadSites();
        void CommitChanges();
        int IndexOf(Site site);
    }
}
