using Client.FileInUse;
using Common;
using Common.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileSending
{
    public class FileSender : IFileSender
    {
        private readonly string path;                                              
        private readonly IDataExport proxy;                                         
        private readonly IFileInUseChecker fileInUseChecker;                       
        public FileSender(IDataExport proxy, IFileInUseChecker fileInUseChecker, string path)
        {
            this.proxy = proxy;
            this.fileInUseChecker = fileInUseChecker;
            this.path = path;
        }

        public void SendFile()                                                      
        {
            SendFile(path);
        }

        public void SendFile(string filePath)                                   
        {
            var fileName = Path.GetFileName(filePath);
            FileData file = new FileData(GetMemoryStream(filePath), fileName);
            proxy.ProcessFile(file);
            file.Dispose();                                                     
        }

        private MemoryStream GetMemoryStream(string filePath)                   
        {
            MemoryStream ms = new MemoryStream();
            if (fileInUseChecker.IsFileInUse(filePath))
            {
                Console.WriteLine($"Cannot process the file {Path.GetFileName(filePath)}. It's being in use by another process or it has been deleted.");
                return ms;
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(ms);
                fileStream.Close();
            }
            return ms;
        }
    }
}
