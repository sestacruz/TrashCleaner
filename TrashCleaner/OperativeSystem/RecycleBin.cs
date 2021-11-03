using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace TrashCleaner.OperativeSystem
{
    public class RecycleBin
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);
        enum RecycleFlags : uint
        {
            SHRB_NOCONFIRMATION = 0x00000001, // Don't ask confirmation
            SHRB_NOPROGRESSUI = 0x00000002, // Don't show any windows dialog
            SHRB_NOSOUND = 0x00000004 // Don't make sound, ninja mode enabled :v
        }

        public static CleanInfo Clear()
        {
            var model = new CleanInfo();
            List<DriveInfo> allDrives = DriveInfo.GetDrives().Where(x=>x.IsReady).ToList();
            long recycleBinSize = 0,
                 recycleBinFiles = 0;
            foreach (var drive in allDrives)
            {
                string recyclePath = $"{drive.Name}$RECYCLE.BIN";
                recycleBinSize += Utils.GetDirSize(recyclePath);
                recycleBinFiles += Directory.GetFiles(recyclePath).Length;
                foreach (string dir in Directory.GetDirectories(recyclePath))
                    recycleBinFiles += Directory.GetFiles(dir).Length;
            }

            model.size = recycleBinSize;
            model.fileCount = recycleBinFiles;

            try
            {
                SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHRB_NOCONFIRMATION);
                model.deletedFiles = model.fileCount;
                model.deletedSize = model.size;
                return model;
            }
            catch (Exception e)
            {
                return model;
                throw e;
            }
        }
    }
}
