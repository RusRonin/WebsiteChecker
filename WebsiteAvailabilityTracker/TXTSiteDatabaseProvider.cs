using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public class TXTSiteDatabaseProvider : ISiteDatabaseProvider
    {
        private List<Site> _sites;
        private string path;

        public TXTSiteDatabaseProvider()
        {
            ConfigureDatabase();

            _sites = new List<Site>();

            ReloadSites();
        }

        private void ConfigureDatabase()
        {
            path = GetPathToFile() + "\\sites.txt";
            CheckFileExistanceOrCreate();
        }

        private string GetPathToFile()
        {
            string filePath;

            try
            {
                var location = Assembly.GetExecutingAssembly().Location;               
                filePath = Path.GetDirectoryName(location);
            }
            catch (Exception)
            {
                filePath = "";
                throw new DatabaseException("Ошибка настройки базы данных");
            }
            return filePath;
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

        private void CheckFileExistanceOrCreate()
        {
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                //FileStream реализует IDisposable, поэтому не возникает ошибки одновременного доступа, как с FileInfo
                //используется пустой блок using, т.к. он имеет более лаконичный синтаксис, чем последовательный вызов методов
            }
        }

        private List<Site> LoadSites()
        {
            List<Site> sites = new List<Site>();

            try
            {
                using (StreamReader streamReader = new StreamReader(path, Encoding.Default))
                {
                    while (streamReader.Peek() != -1)
                    {
                        string fileLine = streamReader.ReadLine();
                        string[] siteParams = fileLine.Split(' ');
                        string address = siteParams[0];
                        uint frequency;
                        if (!UInt32.TryParse(siteParams[1], out frequency))
                        {
                            continue;
                        }
                        Site site = new Site(address, frequency);
                        sites.Add(site);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new DatabaseFileNotFoundException("Файл базы данных не найден");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new DatabaseException("Ошибка чтения базы данных");
            }
            return sites;
        }

        public void SaveSites(List<Site> sites)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.Default))
                {
                    foreach (Site site in sites)
                    {
                        string line = site.Address + " " + site.CheckingFrequency;
                        streamWriter.WriteLine(line);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new DatabaseFileNotFoundException("Файл базы данных не найден");
            }
            catch (Exception)
            {
                throw new DatabaseException("Ошибка записи в базу данных");
            }
        }
    }

    class DatabaseException : Exception
    {
        public DatabaseException(string message)
            : base(message)
        { }
    }

    class DatabaseFileNotFoundException : FileNotFoundException
    {
        public DatabaseFileNotFoundException(string message)
            : base(message)
        { }
    }
}
