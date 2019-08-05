using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WebsiteAvailabilityTracker
{
    public class XMLSiteDatabaseProvider : ISiteDatabaseProvider
    {
        private static XmlDocument xmlDoc;
        private static XmlElement xmlRoot;

        private List<Site> _sites;

        public XMLSiteDatabaseProvider()
        {
            ConfigureDatabase();

            _sites = new List<Site>();

            ReloadSites();
        }

        private void ConfigureDatabase()
        {
            try
            {
                xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load("sites.xml");
                    xmlRoot = xmlDoc.DocumentElement;
                }
                //в случае необнаружения xml файла создается новый.
                //реализовано средствами linq to xml, так как это единственный найденный мною
                //способ избежать необнаружения корневого элемента.
                //этой же цели служит фальшивый адрес и его последущее удаление
                //за счет сверки с пустым списком
                catch (FileNotFoundException)
                {
                    XDocument xdoc = new XDocument(new XElement("sites",
                        new XElement("site",
                            new XAttribute("address", "noaddress"),
                            new XAttribute("frequency", "500")
                        )));
                    xdoc.Save("sites.xml");

                    xmlDoc.Load("sites.xml");
                    xmlRoot = xmlDoc.DocumentElement;
                }
            }
            catch (XmlException)
            {
                throw new DatabaseException("Ошибка XML базы данных");
            }
            catch (Exception)
            {
                throw new DatabaseException("Ошибка базы данных");
            }

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
            _sites = LoadSites();
        }

        public void CommitChanges()
        {
            SaveSites(_sites);
        }

        public int IndexOf(Site site)
        {
            return _sites.IndexOf(site);
        }

        public IEnumerator GetEnumerator()
        {
            return _sites.GetEnumerator();
        }

        public List<Site> GetClonedSiteList()
        {
            List<Site> clonedList = new List<Site>();

            foreach (Site site in _sites)
            {
                clonedList.Add(site);
            }

            return clonedList;
        }

        private List<Site> LoadSites()
        {
            List<Site> sites = new List<Site>() { };
            try
            {
                foreach (XmlNode xmlNode in xmlRoot)
                {
                    if (xmlNode.Name == "site" && xmlNode.Attributes.Count > 0)
                    {
                        XmlNode addressAttribute = xmlNode.Attributes.GetNamedItem("address");
                        XmlNode frequencyAttribute = xmlNode.Attributes.GetNamedItem("frequency");
                        if (addressAttribute != null && frequencyAttribute != null)
                        {
                            uint frequency;
                            if (UInt32.TryParse(frequencyAttribute.Value, out frequency))
                            {
                                Site site = new Site(addressAttribute.Value, frequency);
                                sites.Add(site);
                            }
                        }
                    }
                }
            }
            catch (XmlException)
            {
                throw new DatabaseException("Ошибка XML базы данных");
            }
            catch (ArgumentNullException)
            {
                throw new DatabaseNullArgumentException("Ошибка базы данных");
            }
            catch (Exception)
            {
                throw new DatabaseException("Ошибка базы данных");
            }
            return sites;
        }

        public void SaveSites(List<Site> siteList)
        {
            try
            {
                //копируем список сайтов, чтобы не изменять тот, с которым мы работаем
                List<Site> sites = new List<Site>();
                foreach (Site site in siteList)
                {
                    sites.Add(site);
                }
                //проверяем сайты на устаревание. устаревшие(удаленные из списка) удаляем из документа, 
                //не устаревшие удаляем из вспомогательного списка
                foreach (XmlNode xmlNode in xmlRoot)
                {
                    if (xmlNode.Name == "site" && xmlNode.Attributes.Count > 0)
                    {
                        XmlNode addressAttribute = xmlNode.Attributes.GetNamedItem("address");
                        XmlNode frequencyAttribute = xmlNode.Attributes.GetNamedItem("frequency");
                        if (addressAttribute != null && frequencyAttribute != null)
                        {
                            uint frequency;
                            if (UInt32.TryParse(frequencyAttribute.Value, out frequency))
                            {
                                Site site = new Site(addressAttribute.Value, frequency);
                                if (sites.IndexOf(site) == -1)
                                {
                                    xmlRoot.RemoveChild(xmlNode);
                                }
                                else
                                {
                                    sites.Remove(site);
                                }
                            }
                        }

                    }
                }

                //оставшиеся во вспомогательном списке добавляем в xml документ
                foreach (Site site in sites)
                {
                    XmlElement newSite = xmlDoc.CreateElement("site");
                    XmlAttribute addressAttribute = xmlDoc.CreateAttribute("address");
                    XmlAttribute frequencyAttribute = xmlDoc.CreateAttribute("frequency");
                    XmlText addressValue = xmlDoc.CreateTextNode(site.Address);
                    XmlText frequencyValue = xmlDoc.CreateTextNode(site.CheckingFrequency.ToString());

                    addressAttribute.AppendChild(addressValue);
                    frequencyAttribute.AppendChild(frequencyValue);
                    newSite.Attributes.Append(addressAttribute);
                    newSite.Attributes.Append(frequencyAttribute);
                    xmlRoot.AppendChild(newSite);

                }

                xmlDoc.Save("sites.xml");
            }
            catch (XmlException)
            {
                throw new DatabaseException("Ошибка XML базы данных");
            }
            catch (ArgumentNullException)
            {
                throw new DatabaseNullArgumentException("Ошибка базы данных");
            }
            catch (Exception)
            {
                throw new DatabaseException("Ошибка базы данных");
            }
        }
    }

    class DatabaseNullArgumentException : ArgumentNullException
    {
        public DatabaseNullArgumentException(string message)
            : base(message)
        { }
    }
}