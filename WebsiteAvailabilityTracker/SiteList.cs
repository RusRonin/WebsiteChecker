using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public class SiteList : ISiteList
    {

        private readonly ISiteDatabaseProvider _databaseProvider;
        private List<Site> _sites;
        public SiteList(ISiteDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            _sites = new List<Site>();

            ReloadSites();
        }

        private void CheckSiteCorrectness(Site site)
        {
            //добавить регулярное выражение для проверки адреса
            //заменить 600000 мс (10 минут) на константу
            if (site.CheckingFrequency > 600000)
            {
                throw new SiteCheckingFrequencyException("Неверное значение частоты проверки");
            }
        }

        public void AddSite(Site site)
        {
            CheckSiteCorrectness(site);
            if (!(_sites.IndexOf(site) > -1))
            {
                _sites.Add(site);
            }
        }

        public void RemoveSite(Site site)
        {
            while (_sites.IndexOf(site) > -1)
            {
                _sites.Remove(site);
            }
        }

        public void ReloadSites()
        {
            _sites = _databaseProvider.LoadSites();
        }

        public void CommitChanges()
        {
            _databaseProvider.SaveSites(this);
        }

        public int IndexOf(Site site)
        {
            return _sites.IndexOf(site);
        }

        public IEnumerator GetEnumerator()
        {
            return _sites.GetEnumerator();
        }

        public object Clone()
        {
            SiteList siteList = new SiteList(_databaseProvider);

            foreach (Site site in siteList)
            {
                siteList.RemoveSite(site);
            }
            foreach (Site site in this)
            {
                siteList.AddSite(site);
            }
            return siteList;
        }
    }
}
