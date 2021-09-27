using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TrashCleaner.Chrome
{
    class ChromeCache
    {
        private static string chromeCache = Environment.GetEnvironmentVariable("LocalAppData") + @"\Google\Chrome\User Data\Default\Cache";

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

            if (!Directory.Exists(chromeCache))
                return size;

            List<string> directories = new List<string> { chromeCache };

            foreach (string tempDir in directories)
                size += Utils.GetDirSize(tempDir);

            return size;
        }
        private static long GetTempFilesCount()
        {
            long count = 0;

            if (!Directory.Exists(chromeCache))
                return count;

            List<string> directories = new List<string> { chromeCache };

            foreach (string tempDir in directories)
            {
                count += Directory.GetFiles(tempDir).Length;
                foreach (string dir in Directory.GetDirectories(tempDir))
                    count += Directory.GetFiles(dir).Length;
            }

            return count;
        }
        private static long GetTempDirectoriesCount()
        {
            long count = 0;

            if (!Directory.Exists(chromeCache))
                return count;

            List<string> directories = new List<string> { chromeCache };

            foreach (string tempDir in directories)
            {
                count += Directory.GetDirectories(tempDir).Length;
                foreach (string dir in Directory.GetDirectories(tempDir))
                    count += Directory.GetDirectories(dir).Length;
            }

            return count;
        }

        public static CleanInfo CleanTempFiles()
        {
            var cleanInfo = new CleanInfo
            {
                fileCount = GetTempFilesCount(),
                size = GetTempSize()
            };

            if (!Directory.Exists(chromeCache))
                return cleanInfo;

            List<string> directories = new List<string> { chromeCache };

            foreach (string tempDir in directories)
            {
                Utils.CleanDirectory(tempDir, out long delFiles, out long delSize, out long notDel, out long delFolders);
                cleanInfo.deletedFiles += delFiles;
                cleanInfo.deletedSize += delSize;
                cleanInfo.notDeleted += notDel;
                cleanInfo.deletedFolders += delFolders;
            }
            return cleanInfo;
        }
    }
}
