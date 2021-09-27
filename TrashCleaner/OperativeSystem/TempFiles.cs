using System;
using System.IO;
using System.Collections.Generic;

namespace TrashCleaner.OperativeSystem
{
    public class TempFiles
    {
        private static string systemTempDir = Environment.GetEnvironmentVariable("TEMP",EnvironmentVariableTarget.Machine);
        private static string userTempDir = Environment.GetEnvironmentVariable("TEMP",EnvironmentVariableTarget.User);

        public static long size {
            get { return GetTempSize(); }
        }
        public static long filesCount { 
            get { return GetTempFilesCount(); }
        }
        public static long foldersCount {
            get { return GetTempDirectoriesCount(); }
        }

        private static long GetTempDirectoriesCount()
        {
            long count = 0;

            if (!Directory.Exists(systemTempDir) & !Directory.Exists(userTempDir))
                return count;

            List<string> directories = new List<string> { systemTempDir, userTempDir };

            foreach (string tempDir in directories)
            {
                count += Directory.GetDirectories(tempDir).Length;
                foreach (string dir in Directory.GetDirectories(tempDir))
                    count += Directory.GetDirectories(dir).Length;
            }

            return count;
        }

        private static long GetTempFilesCount()
        {
            long count = 0;

            if (!Directory.Exists(systemTempDir) & !Directory.Exists(userTempDir))
                return count;

            List<string> directories = new List<string> { systemTempDir, userTempDir };

            foreach (string tempDir in directories)
            {
                count += Directory.GetFiles(tempDir).Length;
                foreach (string dir in Directory.GetDirectories(tempDir))
                    count += Directory.GetFiles(dir).Length;
            }

            return count;
        }

        private static long GetTempSize()
        {
            long size = 0;

            if (!Directory.Exists(systemTempDir) & !Directory.Exists(userTempDir))
                return size;

            List<string> directories = new List<string> { systemTempDir, userTempDir };

            foreach(string tempDir in directories)
                size += Utils.GetDirSize(tempDir);

            return size;
        }

        public static CleanInfo CleanTempFiles()
        {
             var cleanInfo = new CleanInfo
            {
                fileCount = GetTempFilesCount(),
                size = GetTempSize()
            };

            if (!Directory.Exists(systemTempDir) & !Directory.Exists(userTempDir))
                return cleanInfo;

            List<string> directories = new List<string> { systemTempDir, userTempDir };

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
