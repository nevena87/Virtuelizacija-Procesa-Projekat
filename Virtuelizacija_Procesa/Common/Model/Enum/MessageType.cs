using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Enum
{
    public enum MessageType
    {
        [EnumMember] Info = 0,
        [EnumMember] Warning = 1,
        [EnumMember] Error = 2
    }
}
