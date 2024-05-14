using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileSending
{
    public interface IFileSender
    {
        void SendFile();
        void SendFile(string filePath);
    }
}
