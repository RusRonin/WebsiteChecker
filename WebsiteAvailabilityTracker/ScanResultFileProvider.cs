using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WebsiteAvailabilityTracker
{
    class ScanResultFileProvider : IScanResultFileProvider
    {
        string path;

        public ScanResultFileProvider()
        {
            path = GetPathToFile() + "\\scanresults.txt";
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

        private void CheckFileExistanceOrCreate()
        {
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                //FileStream реализует IDisposable, поэтому не возникает ошибки одновременного доступа, как с FileInfo
                //используется пустой блок using, т.к. он имеет более лаконичный синтаксис, чем последовательный вызов методов
            }
        }

        public void WriteScanResults(List<SiteResponse> responses)
        {
            CheckFileExistanceOrCreate();
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.Default))
            {
                foreach (SiteResponse response in responses)
                {
                    streamWriter.WriteLine($"{response.GetRespondingSite().Address} - {response.GetStatusCode()} {response.GetStatusCodeText()}");
                }
            }
        }
    }
}
