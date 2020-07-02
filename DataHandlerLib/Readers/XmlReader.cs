using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DataHandlerLib.Models;

namespace DataHandlerLib.Readers
{
    public class XmlReader : IDataReader
    {
        /// <summary>
        /// Reads the entire XML file from the given path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>List<MontorInfo></returns>
        public List<MonitorInfo> ReadData(string path)
        {
            XNamespace ns = @"http://www.nexthink.com/1/api/investigations";
            var monitors = new List<MonitorInfo>();
            var doc = XDocument.Load(path);
            doc.Elements().Descendants().Where(h => h.Name.LocalName == "header").Remove();

            var names = doc.Descendants(ns + "c0").ToList();
            var ips = doc.Descendants(ns + "c2").ToList();
            var count = doc.Descendants(ns + "r").Elements(ns + "c1").ToList();

            for (int i = 0; i < names.Count(); i++)
            {
                var monitor = new MonitorInfo
                {
                    Name = names[i].Value,
                    IPAddress = ips[i].Value,
                    MonitorCount = count[i].Elements(ns + "monitor").Count()
                };
                monitors.Add(monitor);
            }
            return RemoveDuplicates(monitors);

        }

        /// <summary>
        /// Removes duplicates from Monitors list
        /// </summary>
        /// <param name="monitors">Unfiltered list with duplicates</param>
        /// <returns>List<MonitorInfo></returns>
        public List<MonitorInfo> RemoveDuplicates(List<MonitorInfo> monitors)
        {

            var duplicates = monitors.Where(g => g.MonitorCount > 1).ToList();
            foreach (var dup in duplicates)
            {
                monitors.Remove(dup);
            }
            return monitors;
        }
    }
}
