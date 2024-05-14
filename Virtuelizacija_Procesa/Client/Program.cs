using Client.FileFetch;
using Client.FileInUse;
using Client.FileSending;
using Client.FileWatcher;
using Common;
using Common.Interface;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = ConfigurationManager.AppSettings["FilePath"];
            var fileName = ConfigurationManager.AppSettings["FileName"];
            var outputCsvPath = ConfigurationManager.AppSettings["CSVOutputPath"];

            var path = Path.Combine(filePath, fileName);

            ChannelFactory<IDataExport> factory = new ChannelFactory<IDataExport>("DataExportService");
            IDataExport proxy = factory.CreateChannel();

            IUploader uploader = GetUploader(GetFileSender(proxy, GetFileInUseChecker(), path), path);
            IFileFetcher fileFetcher = GetFetcher(proxy, outputCsvPath);
            uploader.Start();

            List<Audit> audits = proxy.GetAudits();
            foreach(Audit a in audits)
            {
                Console.WriteLine($"Audit ID: {a.Id}");
                Console.WriteLine($"Message: {a.Message}");
                Console.WriteLine($"Type: {a.MessageType}");
                Console.WriteLine($"Time stamp: {a.TimeStamp}");

                Console.WriteLine("----------");
            }

            List<ImportedFile> importedFiles = proxy.GetImportedFiles();
            foreach(ImportedFile impFile in importedFiles)
            {
                Console.WriteLine($"Imported file ID: {impFile.Id}");
                Console.WriteLine($"Imported file name: {impFile.FileName}");

                Console.WriteLine("----------");
            }

            do
            {
                fileFetcher.Download();

                Thread.Sleep(15000);
            } while (true);
        }
        private static IFileInUseChecker GetFileInUseChecker()
        {
            return new FileInUseChecker();
        }
        private static IFileSender GetFileSender(IDataExport proxy, IFileInUseChecker fileInUseChecker, string uploadPath)
        {
            return new FileSender(proxy, fileInUseChecker, uploadPath);
        }
        private static IUploader GetUploader(IFileSender fileSender, string uploadPath)
        {
            return new EventUploader(fileSender, uploadPath);
        }
        private static IFileFetcher GetFetcher(IDataExport proxy, string path)
        {
            return new FileFetcher(proxy, path);
        }
    }
}
