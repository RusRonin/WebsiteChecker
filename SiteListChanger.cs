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
    internal static class SiteListChanger
    {
        private static XmlDocument xmlDoc;
        private static XmlElement xmlRoot;

        internal static void ConfigureXml()
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
                            new XAttribute("address", "noaddress")
                        )));
                    xdoc.Save("sites.xml");
                       
                    xmlDoc.Load("sites.xml");
                    xmlRoot = xmlDoc.DocumentElement;
                    List<Site> sites = new List<Site>() { };
                    CommitChanges(sites);
                }
            }
            catch(XmlException)
            {
                UserInterface.PrintString("Возникла ошибка при работе с базой данных. Пожалуйста перезагрузите приложение.", true);
            }
            catch(Exception)
            {
                UserInterface.PrintString("Возникла ошибка. Пожалуйста перезагрузите приложение", true);
            }
            
        }

        internal static void AddSite(ref List<Site> sites)
        {
            UserInterface.PrintString("Введите адрес сайта: ", false);
            string siteAddress = Console.ReadLine();
            Site site = new Site(siteAddress);

            if (sites.IndexOf(site) > -1)
            {
                UserInterface.PrintString("Данный сайт уже есть в списке", true);
            }
            else
            {
                sites.Add(site);
            }
        }

        internal static void RemoveSite(ref List<Site> sites)
        {
            UserInterface.PrintString("Введите адрес сайта: ", false);
            string siteAddress = Console.ReadLine();
            Site site = new Site(siteAddress);

            RemoveSite(ref sites, site);
        }

        internal static void RemoveSite(ref List<Site> sites, Site site)
        {
            while (sites.IndexOf(site) > -1)
            {
                sites.Remove(site);
            }
            UserInterface.PrintString("Данный сайт удален из списка", true);
        }

        internal static List<Site> LoadSites()
        {
            List<Site> sites = new List<Site>() { };
            try
            {
                foreach (XmlNode xmlNode in xmlRoot)
                {
                    if (xmlNode.Name == "site" && xmlNode.Attributes.Count > 0)
                    {
                        XmlNode attr = xmlNode.Attributes.GetNamedItem("address");
                        if (attr != null)
                        {
                            Site site = new Site(attr.Value);
                            sites.Add(site);
                        }
                    }
                }
            }
            catch(XmlException)
            {
                UserInterface.PrintString("Возникла ошибка при работе с базой данных. Пожалуйста перезагрузите приложение.", true);
            }
            catch(ArgumentNullException)
            {
                UserInterface.PrintString("Возникла ошибка. Пожалуйста перезагрузите приложение", true);
            }
            catch(Exception)
            {
                UserInterface.PrintString("Возникла ошибка. Пожалуйста перезагрузите приложение", true);
            }
            return sites;
        }
        internal static void CommitChanges(List<Site> sites)
        {
            try
            {
                //проверяем сайты на устаревание. устаревшие(удаленные из списка) удаляем из документа, 
                //не устаревшие удаляем из вспомогательного списка
                foreach (XmlNode xmlNode in xmlRoot)
                {
                    if (xmlNode.Name == "site" && xmlNode.Attributes.Count > 0)
                    {
                        XmlNode attr = xmlNode.Attributes.GetNamedItem("address");
                        if (attr != null)
                        {
                            Site site = new Site(attr.Value);
                            if (sites.IndexOf(site) == -1)
                            {
                                xmlRoot.RemoveChild(xmlNode);
                            }
                            else
                            {
                                RemoveSite(ref sites, site);
                            }
                        }

                    }
                }

                //оставшиеся во вспомогательном списке добавляем в xml документ
                foreach (Site site in sites)
                {
                    XmlElement newSite = xmlDoc.CreateElement("site");
                    XmlAttribute addressAttribute = xmlDoc.CreateAttribute("address");
                    XmlText addressValue = xmlDoc.CreateTextNode(site.Address);

                    addressAttribute.AppendChild(addressValue);
                    newSite.Attributes.Append(addressAttribute);
                    xmlRoot.AppendChild(newSite);

                }
                
                xmlDoc.Save("sites.xml");
            }
            catch (XmlException)
            {
                UserInterface.PrintString("Возникла ошибка при работе с базой данных. Пожалуйста перезагрузите приложение.", true);
            }
            catch (ArgumentNullException)
            {
                UserInterface.PrintString("Возникла ошибка. Пожалуйста перезагрузите приложение", true);
            }
            catch (Exception)
            {
                UserInterface.PrintString("Возникла ошибка. Пожалуйста перезагрузите приложение", true);
            }
        }
    }
}
