using Client.FileInUse;
using Common;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileFetch
{
    public class FileFetcher : IFileFetcher                      
    {
        private readonly IDataExport proxy;
        private readonly string path;                               
        public FileFetcher(IDataExport proxy, string path)      
        {
            this.proxy = proxy;
            this.path = path;
        }
        public void Download()
        {
            var files = proxy.GetFiles();                       

            foreach (FileData fileData in files)
            {
                DownloadFile(fileData.FileName, fileData.FileStream);
            }
        }
        private void DownloadFile(string fileName, MemoryStream stream)                    
        {
            try
            {
                var fs = new FileStream($"{path}\\{fileName}", FileMode.Create, FileAccess.Write);
                stream.WriteTo(fs);
                fs.Close();
                fs.Dispose();
                stream.Dispose();
                Console.WriteLine($"Fetched file {fileName}, full path: {path}\\{fileName}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
