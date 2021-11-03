using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TrashCleaner
{
    public class Report
    {
        private static List<CleanInfo> cleanInfo = new List<CleanInfo>();
        private static List<Disk> initialDiskState = new List<Disk>();
        private static List<Disk> finalDiskState = new List<Disk>();
        public class Disk
        {
            public string label { get; set; }
            public long totalSpace { get; set; }
            public long occupedSpace { get; set; }
            public long freeSpace { get; set; }
        }  
        public static void AddCleanInfo(CleanInfo data, string description)
        {
            data.description = description;
            cleanInfo.Add(data);
        }
        public static void SetInitialDiskInfo(List<Disk> info)
        {
            initialDiskState = info;
        }
        public static void SetFinalDiskInfo(List<Disk> info)
        {
            finalDiskState = info;
        }
        private static decimal GetFreedSpace(Disk initial, Disk final)
        {
            return decimal.Subtract(final.freeSpace, initial.freeSpace);
        }
        public static string GetDiskInfoTable()
        {
            string table = "";
            ReportDiskTable htmlTable = new ReportDiskTable();
            htmlTable.table = new List<Row>();
            for (int i = 0; i < initialDiskState.Count; i++)
            {
                Row row = new Row { tr = new List<td>()};
                row.tr.Add(new td { value = initialDiskState[i].label });
                row.tr.Add(new td { value = CleanInfo.stringSize(initialDiskState[i].totalSpace) });
                row.tr.Add(new td { value = CleanInfo.stringSize(initialDiskState[i].occupedSpace) });
                row.tr.Add(new td { value = CleanInfo.stringSize(finalDiskState[i].occupedSpace) });
                row.tr.Add(new td { value = CleanInfo.stringSize(initialDiskState[i].freeSpace) });
                row.tr.Add(new td { value = CleanInfo.stringSize(finalDiskState[i].freeSpace) });
                row.tr.Add(new td { value = CleanInfo.stringSize((long)GetFreedSpace(initialDiskState[i], finalDiskState[i])) });
                htmlTable.table.Add(row);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ReportDiskTable));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, htmlTable);
                table = textWriter.ToString();
            }

            /*SACAR ESTE ASCOOOOOO*/
            table = table.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", string.Empty);
            table = table.Replace("<ReportDiskTable xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", string.Empty);
            table = table.Replace("<table>", string.Empty);
            table = table.Replace("<Row>", string.Empty);
            table = table.Replace("</Row>", string.Empty);
            table = table.Replace("</table>", string.Empty);
            table = table.Replace("</ReportDiskTable>", string.Empty);

            return table;
        }

        public static string GetHtmlReport()
        {
            string htmlReport = File.ReadAllText(@".\Mail\Template\Template.html");
            htmlReport = htmlReport.Replace("@MachineName", Environment.MachineName);
            htmlReport = htmlReport.Replace("@DateTime", DateTime.Now.ToString("dd/MM/yyyy"));
            htmlReport = htmlReport.Replace("@TimePowerOn", OperativeSystem.TimePowerOn.GetTimePowerOn());
            foreach (var item in cleanInfo)
                htmlReport = htmlReport.Replace(item.description, item.ToReportString());
            htmlReport = htmlReport.Replace("@TableContent", GetDiskInfoTable());
            return htmlReport;
        }
    }

    public class ReportDiskTable
    {
        [XmlArray("table")]
        public List<Row> table;
    }
    public class Row
    {
        [XmlArray("tr")]
        public List<td> tr;
    }
    public class td
    {
        [XmlAttribute]
        public string colspan = string.Empty;
        [XmlAttribute]
        public string rowspan = string.Empty;
        [XmlAttribute]
        public string style = "border: 1px solid #ffffff;padding: 8px;";
        [XmlText]
        public string value;
    }

    public class CleanInfo
    {
        public long fileCount { get; set; }
        public long folderCount { get; set; }
        public long size { get; set; }
        public long deletedFiles { get; set; }
        public long deletedFolders { get; set; }
        public long deletedSize { get; set; }
        public long notDeleted { get; set; }
        public string description { get; set; }

        public string ToReportString()
        {
            return $"{fileCount} archivos encontrados en {folderCount} carpetas que representan {stringSize(size)} </br>" +
                   $"{deletedFiles} archivos eliminados y {deletedFolders} carpetas eliminadas, liberando {stringSize(deletedSize)} </br>" +
                   $"{notDeleted} archivos no pudieron ser eliminados";
        }

        private static readonly string[] sizes = { "Bytes", "KBytes", "MBytes", "GBytes", "TBytes", "PBytes" };

        public static string stringSize(long size)
        {
            int order = 0;
            decimal dSize = size;
            while (dSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                dSize = decimal.Divide(dSize, 1024);
            }

            return string.Format("{0:0.##} {1}", dSize, sizes[order]);
        }
    }
}
