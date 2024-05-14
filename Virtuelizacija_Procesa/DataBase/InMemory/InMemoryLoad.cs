using Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.InMemory
{
    public class InMemoryLoad                              
    {
        private InMemoryLoad() { }

        private static readonly Lazy<InMemoryLoad> lazyInstance = new Lazy<InMemoryLoad>(() =>
        {
            return new InMemoryLoad();
        });

        public static InMemoryLoad Instance                 
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        private ConcurrentDictionary<int, Load> dB = new ConcurrentDictionary<int, Load>();                 

        public Dictionary<int, Load> GetLoadData()             
        {
            List<int> keys = dB.Keys.ToList();
            Dictionary<int, Load> loadData = new Dictionary<int, Load>();
            keys.ForEach(t => AddToDict(loadData, t));
            return loadData;
        }

        private void AddToDict(Dictionary<int, Load> loadData, int l)
        {
            Load load;
            if (dB.TryGetValue(l, out load))
            {
                loadData.Add(l, load);
            }
        }

        public bool InsertLoad(int id, Load load)
        {
            return dB.TryAdd(id, load);
        }
        public Load GetLoadById(int id)
        {
            Load load;
            if (dB.TryGetValue(id, out load))
            {
                return load;
            }
            return null;
        }

        public Load GetLoadByTimeStamp(DateTime dateTime)
        {
            return dB.Values.FirstOrDefault(l => l.TimeStamp == dateTime);

        }

        public void UpdateLoad(int id, Load updatedLoad)
        {
            Load load = GetLoadByTimeStamp(updatedLoad.TimeStamp);         

            if (load != null)
            {
                dB[id] = updatedLoad;
            }
            else
            {
                InsertLoad(id, updatedLoad);
            }
        }
    }
}
