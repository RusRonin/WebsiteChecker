using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    interface ICommandUserInterface : IUserInterface
    {
        void PrintCommandList();
        void PrintSites(ISiteList sites);
        void ReadCommand(ISiteList sites, ref bool endWork);
    }
}
