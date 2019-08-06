using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteAvailabilityTracker
{
    public interface ISiteChecker
    {
        void CheckSites(ISiteDatabaseProvider sites, CancellationToken token);
        SiteResponse CheckSite(Site site);
        Task<SiteResponse> CheckSiteAsync(Site site);
        void CheckSitesAsync(ISiteDatabaseProvider sites, CancellationToken token);
    }
}
