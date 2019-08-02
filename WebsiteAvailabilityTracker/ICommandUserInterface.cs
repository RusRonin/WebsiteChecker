using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebsiteAvailabilityTracker
{
    interface ICommandUserInterface : IUserInterface
    {
        void PrintCommandList();
        void PrintSites(ISiteList sites);
        void ReadCommand(ISiteList sites, CancellationTokenSource cts, ref bool endWork, ref bool asyncCheckRestart);
    }
}
