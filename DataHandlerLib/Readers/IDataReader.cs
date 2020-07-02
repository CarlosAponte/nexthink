using System.Collections.Generic;
using DataHandlerLib.Models;

namespace DataHandlerLib.Readers
{
    public interface IDataReader
    {
        List<MonitorInfo> ReadData(string path);

        List<MonitorInfo> RemoveDuplicates(List<MonitorInfo> monitors);
    }
}
