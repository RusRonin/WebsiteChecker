using System;
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

        public XMLSiteDatabaseProvider()
        {
            ConfigureDatabase();
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

        public List<Site> LoadSites()
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

        public void SaveSites(ISiteList siteList)
        {
            try
            {
                //копируем список сайтов, чтобы не изменять тот, с которым мы работаем
                ISiteList sites = (ISiteList) siteList.Clone();
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
                                    sites.RemoveSite(site);
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