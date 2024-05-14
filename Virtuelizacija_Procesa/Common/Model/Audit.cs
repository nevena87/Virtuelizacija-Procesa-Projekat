using Common.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    [DataContract]
    public class Audit
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public MessageType MessageType { get; set; }
        [DataMember]
        public string Message { get; set; }

        static int counter = 1;

        public Audit() { }
        public Audit(DateTime timeStamp, MessageType messageType, string message)
        {
            Id = counter++;
            TimeStamp = timeStamp;
            MessageType = messageType;
            Message = message;
        }
    }
}
