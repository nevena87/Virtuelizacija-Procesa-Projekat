using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileInUse
{
    public interface IFileInUseChecker
    {
        bool IsFileInUse(string filePath);
    }
}
