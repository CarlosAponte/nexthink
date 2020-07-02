using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataHandlerLib.Models;

namespace DataHandlerLib.Readers
{
    public class CsvReader
    {
        /// <summary>
        /// Reads entire file from path
        /// </summary>
        /// <param name="path">The path where the files is being loaded</param>
        /// <returns>List<MonitorInfo></returns>
        public List<MonitorInfo> ReadData(string path)
        {
            var monitors = File.ReadLines(path)
                .Skip(1)
                .Select(l => ReadCsvLine(l))
                .ToList();
            return RemoveDuplicates(monitors);
        }

        /// <summary>
        /// Will remove duplicate information based on the name
        /// </summary>
        /// <param name="monitors">Unfiltered list of List<MonitorInfo></param>
        /// <returns>List<MonitorInfo></returns>
        public List<MonitorInfo> RemoveDuplicates(List<MonitorInfo> monitors)
        {
            var duplicates = monitors.GroupBy(x => x.Name).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            foreach (var dup in duplicates)
            {
                monitors.RemoveAll(m => m.Name == dup);
            }
            return monitors;
        }

        /// <summary>
        /// Splits into single information the lines readed from the CSV file
        /// </summary>
        /// <param name="line">Line from file</param>
        /// <returns>MonitorInfo</returns>
        private MonitorInfo ReadCsvLine(string line)
        {
            var values = line.Split(';');
            var monitorInfo = new MonitorInfo()
            {
                Name = values[0],
                IPAddress = values[1],
            };
            return monitorInfo;
        }
    }
}
