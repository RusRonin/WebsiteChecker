using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebsiteChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            SiteListChanger.ConfigureXml();
            List<Site> sites = SiteListChanger.LoadSites();

            PrintCommandList();
            bool endWork = false;
            while (!endWork)
            {
                string command = Console.ReadLine();
                switch(command.ToLower())
                {
                    case "добавить":
                        SiteListChanger.AddSite(ref sites);
                        break;
                    case "удалить":
                        SiteListChanger.RemoveSite(ref sites);
                        break;
                    case "сохранить":
                        SiteListChanger.CommitChanges(sites);
                        break;
                    case "сброс":
                        sites = SiteListChanger.LoadSites();
                        break;
                    case "печать":
                        PrintSites(sites);
                        break;
                    case "выключение":
                        endWork = true;
                        break;
                    case "справка":
                        PrintCommandList();
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда. Проверьте правильность ввода или введите \"Справка\" для просмотра списка команд");
                        break;
                }
            }
        }

        static void PrintCommandList()
        {
            Console.WriteLine("Введите команду из следующего списка: ");
            Console.WriteLine("- Добавить :для добавления сайта в список");
            Console.WriteLine("- Удалить :для удаления сайта из списка");
            Console.WriteLine("- Сохранить :для сохранения внесенных изменений");
            Console.WriteLine("- Сброс :для сброса изменений (отката к последнему сохранению)");
            Console.WriteLine("- Печать :для вывода списка сайтов");
            Console.WriteLine("- Выключение :для завершения работы с программой");
            Console.WriteLine("- Справка :для повторного вывода списка команд");
        }

        static void PrintSites(List<Site> sites)
        {
            Console.WriteLine("Список сайтов: ");
            foreach (Site site in sites)
            {
                Console.WriteLine("- {0}", site.Address);
            }
        }
    }

    internal static class SiteListChanger
    {
        private static XmlDocument xmlDoc;
        private static XmlElement xmlRoot;

        internal static void ConfigureXml()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load("sites.xml");
            xmlRoot = xmlDoc.DocumentElement;
        }

        internal static void AddSite(ref List<Site> sites)
        {
            Console.Write("Введите адрес сайта: ");
            string siteAddress = Console.ReadLine();
            Site site = new Site(siteAddress);

            if (sites.IndexOf(site) > -1)
            {
                Console.WriteLine("Данный сайт уже есть в списке");
            }
            else
            {
                sites.Add(site);
            }
        }

        internal static void RemoveSite(ref List<Site> sites)
        {
            Console.Write("Введите адрес сайта: ");
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
            Console.WriteLine("Данный сайт удален из списка");
        }

        internal static List<Site> LoadSites()
        {
            List<Site> sites = new List<Site>() { };
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
            return sites;
        }
        internal static void CommitChanges(List<Site> sites)
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
    }

    class Site :IComparable<Site>, IComparer<Site>, IEquatable<Site>
    {
        public string Address { get; set; }
        
        public Site(string siteAddress)
        {
            Address = siteAddress;
        }

        /*public static bool operator ==(Site s1, Site s2)
        {
            return s1.Address == s2.Address;
        }

        public static bool operator !=(Site s1, Site s2)
        {
            return s1.Address != s2.Address;
        }*/

        public int CompareTo(Site s)
        {
            return this.Address.CompareTo(s.Address);
        }

        public int Compare(Site s1, Site s2)
        {
            return String.Compare(s1.Address, s2.Address);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Site);
        }

        public bool Equals(Site s)
        {
            return s != null && this.Address == s.Address;
        }
    }
}
