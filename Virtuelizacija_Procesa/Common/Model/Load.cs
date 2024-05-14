using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    [DataContract]
    public class Load
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public float ForecastValue { get; set; }
        [DataMember]
        public float MeasuredValue { get; set; }
        [DataMember]
        public float AbsolutePercentageDeviation { get; set; }
        [DataMember]
        public float SquaredDeviation { get; set; }
        [DataMember]
        public int ImportedFileId { get; set; }

        static int counter = 1;

        public Load() { }

        public Load(DateTime timeStamp, float forecastValue, float measuredValue)
        {
            Id = counter++;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
        }
    }
}
