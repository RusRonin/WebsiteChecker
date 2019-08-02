using System;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public interface IUserInterface
    {
        void PrintString(string text, bool withWrap);
        string ReadLine();
    }
}
