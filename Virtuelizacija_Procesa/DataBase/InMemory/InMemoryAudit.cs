using Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.InMemory
{
    public class InMemoryAudit
    {
        private InMemoryAudit() { }

        private static readonly Lazy<InMemoryAudit> lazyInstance = new Lazy<InMemoryAudit>(() =>
        {
            return new InMemoryAudit();
        });

        public static InMemoryAudit Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        private ConcurrentDictionary<int, Audit> dB = new ConcurrentDictionary<int, Audit>();

        public Dictionary<int, Audit> GetAuditData()
        {
            List<int> keys = dB.Keys.ToList();
            Dictionary<int, Audit> auditData = new Dictionary<int, Audit>();
            keys.ForEach(t => AddToDict(auditData, t));
            return auditData;
        }

        private void AddToDict(Dictionary<int, Audit> auditData, int l)
        {
            Audit audit;
            if (dB.TryGetValue(l, out audit))
            {
                auditData.Add(l, audit);
            }
        }

        public bool InsertAudit(int id, Audit audit)
        {
            return dB.TryAdd(id, audit);
        }
        public Audit GetAuditById(int id)
        {
            Audit audit;
            if (dB.TryGetValue(id, out audit))
            {
                return audit;
            }
            return null;
        }
        public bool UpdateAudit(int id, Audit updatedAudit)
        {
            if (dB.ContainsKey(id))
            {
                dB[id] = updatedAudit;
                return true;
            }
            return false;
        }

    }
}
