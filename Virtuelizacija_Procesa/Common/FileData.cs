using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class FileData                                   //klasa kojom se razmijenjuju podaci izmedju servera i klijenta
    {
        [DataMember]
        public MemoryStream FileStream { get; set; }

        [DataMember]
        public string FileName { get; set; }

        public FileData(MemoryStream fileStream, string fileName)
        {
            FileStream = fileStream;
            FileName = fileName;
        }

        public void Dispose()
        {
            if (FileStream != null)
            {
                try
                {
                    FileStream.Dispose();
                    FileStream.Close();
                    FileStream = null;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error with file {FileName}");
                }
            }
        }
    }
}
