using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteAvailabilityTracker
{
    public class SiteChecker : ISiteChecker
    {
        private IScanResultFileProvider _scanResultFileProvider;

        public SiteChecker(IScanResultFileProvider scanResultFileProvider)
        {
            _scanResultFileProvider = scanResultFileProvider;
        }

        public void CheckSites(ISiteList sites, CancellationToken token)
        {
            List<SiteResponse> responses = new List<SiteResponse>();
            DateTime currentDateTime;
            DateTime previousDateTime = DateTime.Now;
            while (!token.IsCancellationRequested)
            {
                currentDateTime = DateTime.Now;
                TimeSpan passedTime = (currentDateTime - previousDateTime);
                uint passedMilliseconds = (uint) passedTime.TotalMilliseconds;
                foreach (Site site in sites)
                {
                    if (site.NeedToBeChecked(passedMilliseconds))
                    {
                        SiteResponse siteResponse = new SiteResponse(site);
                        if (responses.IndexOf(siteResponse) > -1)
                        {
                            //т.к. сравнение происходит только по сайту, здесь удаляется прошлый ответ от этого сайта и записывается новый
                            responses.Remove(siteResponse);
                        }
                        responses.Add(siteResponse);
                    }
                }
                _scanResultFileProvider.WriteScanResults(responses);
                Thread.Sleep(500);
            }
        }

        public SiteResponse CheckSite(Site site)
        {
            return new SiteResponse(site);
        }

        public async Task<SiteResponse> CheckSiteAsync(Site site)
        {
            return await Task.Run(() => CheckSite(site));
        }

        public async void CheckSitesAsync(ISiteList sites, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                await Task.Run(() => CheckSites(sites, token));
            }          
        }
    }
}
