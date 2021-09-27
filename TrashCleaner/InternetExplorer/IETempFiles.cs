using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashCleaner.InternetExplorer
{
    public class IETempFiles
    {
        private static string w8_10 = Environment.GetEnvironmentVariable("LocalAppData") + @"\Microsoft\Windows\INetCache";
        private static string wVista_7 = Environment.GetEnvironmentVariable("LocalAppData") + @"\Microsoft\Windows\Temporary Internet Files";

        public static long size
        {
            get { return GetTempSize(); }
        }
        public static long filesCount
        {
            get { return GetTempFilesCount(); }
        }
        public static long foldersCount
        {
            get { return GetTempDirectoriesCount(); }
        }

        private static long GetTempSize()
        {
            long size = 0;

            if (!Directory.Exists(w8_10) && !Directory.Exists(wVista_7))
                return size;

            List<string> directories = new List<string> { w8_10, wVista_7 };
            try
            {
                foreach (string tempDir in directories)
                    foreach (string dir in Directory.GetDirectories(tempDir))
                    {
                        try
                        {
                            size += Utils.GetDirSize(dir);
                        }
                        catch (Exception) { }
                    }
            }
            catch (Exception) { }
            return size;
        }
        private static long GetTempFilesCount()
        {
            long count = 0;

            if (!Directory.Exists(w8_10) && !Directory.Exists(wVista_7))
                return count;

            List<string> directories = new List<string> { w8_10, wVista_7 };
            foreach (string tempDir in directories)
            {
                try
                {
                    count += Directory.GetFiles(tempDir).Length;
                }
                catch (Exception) { }
            }

            return count;
        }
        private static long GetTempDirectoriesCount()
        {
            long count = 0;

            if (!Directory.Exists(w8_10) && !Directory.Exists(wVista_7))
                return count;

            List<string> directories = new List<string> { w8_10, wVista_7 };

            foreach (string tempDir in directories)
                count += Directory.GetDirectories(tempDir).Length;

            return count;
        }

        public static CleanInfo CleanTempFiles()
        {
            var cleanInfo = new CleanInfo
            {
                fileCount = GetTempFilesCount(),
                size = GetTempSize()
            };

            if (!Directory.Exists(w8_10) && !Directory.Exists(wVista_7))
                return cleanInfo;

            List<string> directories = new List<string> { w8_10, wVista_7 };

            foreach (string tempDir in directories)
            {
                try
                {
                    Utils.CleanDirectory(tempDir, out long delFiles, out long delSize, out long notDel, out long delFolders);
                    cleanInfo.deletedFiles += delFiles;
                    cleanInfo.deletedSize += delSize;
                    cleanInfo.notDeleted += notDel;
                    cleanInfo.deletedFolders += delFolders;
                }
                catch (Exception) { }
            }
            return cleanInfo;
        }
    }
}
