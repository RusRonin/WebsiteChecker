using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public interface ISiteChecker
    {
        void CheckSite(Site site);
    }
}
