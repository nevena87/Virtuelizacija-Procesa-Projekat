using Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.InMemory
{
    public class InMemoryImpFile
    {
        private InMemoryImpFile() { }

        private static readonly Lazy<InMemoryImpFile> lazyInstance = new Lazy<InMemoryImpFile>(() =>
        {
            return new InMemoryImpFile();
        });

        public static InMemoryImpFile Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        private ConcurrentDictionary<int, ImportedFile> dB = new ConcurrentDictionary<int, ImportedFile>();

        public Dictionary<int, ImportedFile> GetImportedFileData()
        {
            List<int> keys = dB.Keys.ToList();
            Dictionary<int, ImportedFile> importedFileData = new Dictionary<int, ImportedFile>();
            keys.ForEach(t => AddToDict(importedFileData, t));
            return importedFileData;
        }

        private void AddToDict(Dictionary<int, ImportedFile> importedFileData, int l)
        {
            ImportedFile importedFile;
            if (dB.TryGetValue(l, out importedFile))
            {
                importedFileData.Add(l, importedFile);
            }
        }

        public bool InsertImportedFile(int id, ImportedFile importedFile)
        {
            return dB.TryAdd(id, importedFile);
        }
        public ImportedFile GetImportedFileById(int id)
        {
            ImportedFile importedFile;
            if (dB.TryGetValue(id, out importedFile))
            {
                return importedFile;
            }
            return null;
        }
        public bool UpdateimportedFile(int id, ImportedFile importedFile)
        {
            if (dB.ContainsKey(id))
            {
                dB[id] = importedFile;
                return true;
            }
            return false;
        }
    }
}
