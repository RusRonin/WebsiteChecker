using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    public class TXTSiteDatabaseProvider : ISiteDatabaseProvider
    {
        private string path;

        public TXTSiteDatabaseProvider()
        {
            ConfigureDatabase();
        }

        private void ConfigureDatabase()
        {
            path = GetPathToFile() + "site.txt";
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
                throw new TxtDatabaseException("Ошибка настройки базы данных");
            }

            return filePath;
        }

        private void CheckFileExistanceOrCreate()
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
        }

        public List<Site> LoadSites()
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
                throw new TxtDbFileNotFoundException("Файл базы данных не найден");
            }
            catch (Exception)
            {
                throw new TxtDatabaseException("Ошибка чтения базы данных");
            }

            return sites;
        }

        public void SaveSites(ISiteList sites)
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
                throw new TxtDbFileNotFoundException("Файл базы данных не найден");
            }
            catch (Exception)
            {
                throw new TxtDatabaseException("Ошибка записи в базу данных");
            }
        }
    }

    class TxtDatabaseException : Exception
    {
        public TxtDatabaseException(string message)
            : base(message)
        { }
    }

    class TxtDbFileNotFoundException : FileNotFoundException
    {
        public TxtDbFileNotFoundException(string message)
            : base(message)
        { }
    }
}
