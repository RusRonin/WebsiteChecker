using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public interface ISiteList : IEnumerable, ICloneable
    {
        void AddSite(Site site);
        void RemoveSite(Site site);
        void ReloadSites();
        void CommitChanges();
        int IndexOf(Site site);
    }
}
