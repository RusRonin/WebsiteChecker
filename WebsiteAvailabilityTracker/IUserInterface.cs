using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebsiteAvailabilityTracker
{
    public interface IUserInterface
    {
        void PrintString(string text, bool withWrap);
        string ReadLine();
        void PrintCommandList();
        void PrintSites(ISiteDatabaseProvider sites);
        void ReadCommand(ISiteDatabaseProvider sites, CancellationTokenSource cts, ref bool endWork, ref bool asyncCheckRestart);
    }
}
