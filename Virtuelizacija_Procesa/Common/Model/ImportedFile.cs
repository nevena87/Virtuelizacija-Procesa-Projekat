using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    [DataContract]
    public class ImportedFile
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string FileName { get; set; }

        static int counter = 1;

        public ImportedFile() { }
        public ImportedFile(string fileName)
        {
            Id = counter++;
            FileName = fileName;
        }
    }
}
