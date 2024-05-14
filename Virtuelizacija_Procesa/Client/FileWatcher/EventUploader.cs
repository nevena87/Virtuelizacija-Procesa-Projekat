using Client.FileFetch;
using Client.FileSending;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileWatcher
{
    public class EventUploader :IUploader                        
    {                                                            
        private FileSystemWatcher inputFileWatcher;
        private readonly IFileSender fileSender;
        public EventUploader(IFileSender fileSender, string path)
        {
            CreateFileSystemWatcher(path);
            this.fileSender = fileSender;
        }

        private void FileCreated(object sender, FileSystemEventArgs e)  
        {
            try
            {
                fileSender.SendFile();                         
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void FileChanged(object sender, FileSystemEventArgs e)     
        {
            try
            {
                //Console.WriteLine("changed");
                fileSender.SendFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void CreateFileSystemWatcher(string path)           
        {
            var directory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);

            inputFileWatcher = new FileSystemWatcher(directory)
            {
                IncludeSubdirectories = false,
                InternalBufferSize = 32768, 
                Filter = "*.xml*",                                  
            };
            inputFileWatcher.Created += FileCreated;                
            inputFileWatcher.Changed += FileChanged;
        }

        public void Start()
        {
            fileSender.SendFile();                                  
            inputFileWatcher.EnableRaisingEvents = true;            
        }
    }
}
