using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public interface IScanResultFileProvider
    {
        void WriteScanResults(List<SiteResponse> responses);
    }
}
