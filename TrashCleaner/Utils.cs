using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TrashCleaner
{
    public class Utils
    {
        private static List<long> origFreeSpace = new List<long>();

        public static List<Report.Disk> GetDisks()
        {
            var driveInfo = DriveInfo.GetDrives();
            var disksInfo = new List<Report.Disk>();
            for (int i = 0; i < driveInfo.Length; i++)
            {
                Report.Disk disk = new Report.Disk
                {
                    label = $"{driveInfo[i].VolumeLabel} ({driveInfo[i].Name})",
                    totalSpace = driveInfo[i].TotalSize,
                    freeSpace = driveInfo[i].TotalFreeSpace,
                    occupedSpace = driveInfo[i].TotalSize - driveInfo[i].TotalFreeSpace
                };
                disksInfo.Add(disk);
            }
            return disksInfo;
        }

        public static void SetFreeSpace()
        {
            var driveInfo = DriveInfo.GetDrives();
            for (int i = 0; i < driveInfo.Length; i++)
            {
                origFreeSpace.Add(driveInfo[i].TotalFreeSpace);
            }
        }
        public static string PrintDrivesData()
        {
            string print = "-------------------------------------------" + Environment.NewLine;
            foreach (var drive in DriveInfo.GetDrives())
            {
                print += $"\tUnidad {drive.Name} ({drive.VolumeLabel})" + Environment.NewLine +
                         $"\tEspacio total:      {CleanInfo.stringSize(drive.TotalSize)}" + Environment.NewLine +
                         $"\tEspacio disponible: {CleanInfo.stringSize(drive.TotalFreeSpace)}" + Environment.NewLine + Environment.NewLine;
            }
            print += "-------------------------------------------" + Environment.NewLine;
            return print;
        }
        public static string PrintDrivesStat()
        {
            DriveInfo[] newDriveInfo = DriveInfo.GetDrives();
            string print = Environment.NewLine;
            for (int i = 0; i < newDriveInfo.Length; i++)
            {
                long originalFreeSpace = origFreeSpace[i];
                long newFreeSpace = newDriveInfo[i].TotalFreeSpace;
                print += $"\tEspacio liberado en la unidad {newDriveInfo[i].Name} = {CleanInfo.stringSize(newFreeSpace - originalFreeSpace)}";
                print += Environment.NewLine;
            }
            return print;
        }


        public static string PrintEraseDate(CleanInfo model)
        {
            return $"Eliminados {model.deletedFiles} archivos de {model.fileCount} encontrados.{Environment.NewLine}" +
                   $"Eliminados {model.deletedFolders} directorios.{Environment.NewLine}" +
                   $"Liberados {CleanInfo.stringSize(model.deletedSize)}ytes de {CleanInfo.stringSize(model.size)}ytes.{Environment.NewLine}" +
                   $"No se pudieron eliminar {model.notDeleted} archivos{Environment.NewLine}{Environment.NewLine}";
        }

        public static long GetDirSize(string directory)
        {
            long size = 0;

            foreach (string subDir in Directory.GetDirectories(directory))
                size += GetDirSize(subDir);
            
            foreach (string file in Directory.GetFiles(directory))
                size += GetFileSize(file);

            return size;
        }

        public static long GetFileSize(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    FileInfo finfo = new FileInfo(file);
                    return finfo.Length;
                }
            }
            catch(Exception)
            {
                return 0;
            }
            return 0;
        }

        public static void CleanDirectory(string directory, out long deletedFiles, out long deletedSize, out long notDeleted, out long deletedFolders)
        {
            deletedFiles = 0;
            deletedSize = 0;
            notDeleted = 0;
            deletedFolders = 0;

            CleanDirectoryFiles(directory, out long delFiles, out long delSize, out long notDel);
            deletedFiles += delFiles;
            deletedSize += delSize;
            notDeleted += notDel;

            foreach (string subDir in Directory.GetDirectories(directory))
            {
                CleanDirectory(subDir, out delFiles, out delSize, out notDel, out long delFolders);
                deletedFiles += delFiles;
                deletedSize += delSize;
                notDeleted += notDel;
                deletedFolders += delFolders;
            }

            if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
            {
                try
                {
                    Directory.Delete(directory);
                    deletedFolders += 1;
                }
                catch (Exception) { }
            }
        }

        public static void CleanDirectoryFiles(string directory, out long deletedFiles, out long deletedSize, out long notDeleted)
        {
            deletedFiles = 0;
            deletedSize = 0;
            notDeleted = 0;
            foreach (var file in Directory.GetFiles(directory))
            {
                try
                {
                    FileInfo finfo = new FileInfo(file);
                    long tempSize = finfo.Length;
                    File.Delete(file);
                    deletedFiles += 1;
                    deletedSize += tempSize;
                }
                catch (Exception)
                {
                    notDeleted += 1;
                }
            }
        }
    }
}
