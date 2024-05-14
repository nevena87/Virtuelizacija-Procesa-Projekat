using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    [ServiceContract]
    public interface IDataExport                            
    {
        [OperationContract]
        void ProcessFile(FileData fileData);
        [OperationContract]
        List<FileData> GetFiles();
        [OperationContract]
        List<Audit> GetAudits();
        [OperationContract]
        List<ImportedFile> GetImportedFiles();
    }
}
