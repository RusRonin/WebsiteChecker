using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WebsiteChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            SiteListChanger.ConfigureXml();
            List<Site> sites = SiteListChanger.LoadSites();

            UserInterface.PrintCommandList();
            bool endWork = false;
            while (!endWork)
            {
                UserInterface.ReadCommand(ref sites, ref endWork);
            }
        }

        
    }       
}
