using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WebsiteAvailabilityTracker
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
                            new XAttribute("address", "noaddress"),
                            new XAttribute("frequency", "500")
                        )));
                    xdoc.Save("sites.xml");

                    xmlDoc.Load("sites.xml");
                    xmlRoot = xmlDoc.DocumentElement;
                    List<Site> sites = new List<Site>() { };
                    CommitChanges(sites);
                }
            }
            catch (XmlException)
            {
                UserInterface.PrintString("Возникла ошибка при работе с базой данных. Пожалуйста перезагрузите приложение.", true);
            }
            catch (Exception)
            {
                UserInterface.PrintString("Возникла ошибка. Пожалуйста перезагрузите приложение", true);
            }

        }

        internal static void AddSite(ref List<Site> sites)
        {
            Site site;
            while (true)
            {
                UserInterface.PrintString("Введите адрес сайта: ", false);
                string siteAddress = UserInterface.ReadLine();
                UserInterface.PrintString(
                    $"Введите частоту проверки в миллисекундах с шагом {SiteChecker.GetCheckingTimeStep()}" +
                    $" ({SiteChecker.GetCheckingTimeStep()}, {SiteChecker.GetCheckingTimeStep() * 2}...):", false);
                string enteredFrequency = UserInterface.ReadLine();
                uint siteCheckingFrequency;
                bool successfulParse = UInt32.TryParse(enteredFrequency, out siteCheckingFrequency);
                if (!successfulParse)
                {
                    UserInterface.PrintString("Некорректный ввод. Попробуйте еще раз.", true);
                    continue;
                }
                try
                {
                    site = new Site(siteAddress, siteCheckingFrequency);
                    break;
                }
                catch (SiteCheckingFrequencyException e)
                {
                    UserInterface.PrintString("Некорректное значение частоты проверки." +
                        $" Частота должна быть целым положительным числом, кратным {SiteChecker.GetCheckingTimeStep()}", true);
                    continue;
                }
                catch (Exception e)
                {
                    UserInterface.PrintString("Введенные данные содержат ошибку." +
                        " Пожалуйста попробуйте еще раз.", true);
                    continue;
                }
            }
            

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
            //частота не влияет ни на что, так как Equals сравнивает только по адресам
            Site site = new Site(siteAddress, (uint) SiteChecker.GetCheckingTimeStep());

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
                                    RemoveSite(ref sites, site);
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