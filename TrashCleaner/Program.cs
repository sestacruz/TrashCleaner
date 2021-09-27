using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashCleaner.OperativeSystem;
using TrashCleaner.InternetExplorer;
using TrashCleaner.Chrome;
using TrashCleaner.Log;
using TrashCleaner.Mail;

namespace TrashCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var defaultColor = Console.ForegroundColor;
            try
            {
                Utils.SetFreeSpace();
                Report.SetInitialDiskInfo(Utils.GetDisks());
                Console.WriteLine(Utils.PrintDrivesData());
                Logger.WriteLogFile(Utils.PrintDrivesData());
                //TEMP
                Console.Write("Limpieza de archivos temporales de Windows...\t\t");
                Logger.WriteLogFile("Limpieza de archivos temporales de Windows");
                DateTime inTime = DateTime.Now;
                var tempInfo = TempFiles.CleanTempFiles();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
                Console.ForegroundColor = defaultColor;
                Report.AddCleanInfo(tempInfo, "@StatusWindowsTemp");
                Logger.WriteLogFile(Utils.PrintEraseDate(tempInfo));

                //IE TEMP
                Console.Write("Limpieza de archivos temporales de Internet Explorer...\t");
                Logger.WriteLogFile("Limpieza de archivos temporales de Internet Explorer");
                tempInfo = IETempFiles.CleanTempFiles();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
                Console.ForegroundColor = defaultColor;
                Report.AddCleanInfo(tempInfo, "@StatusIETemp");
                Logger.WriteLogFile(Utils.PrintEraseDate(tempInfo));

                //CHROME CACHE
                Console.Write("Limpieza cache de Google Chrome...\t\t\t");
                Logger.WriteLogFile("Limpieza cache de Google Chrome");
                tempInfo = ChromeCache.CleanTempFiles();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
                Console.ForegroundColor = defaultColor;
                Report.AddCleanInfo(tempInfo, "@StatusChromeTemp");
                Logger.WriteLogFile(Utils.PrintEraseDate(tempInfo));

                //RECYCLE BIN
                Console.Write("Vaciado de papelera de reciclaje...\t\t\t");
                Logger.WriteLogFile("Vaciado de papelera de reciclaje");
                tempInfo = RecycleBin.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
                Console.ForegroundColor = defaultColor;
                Report.AddCleanInfo(tempInfo, "@StatusRecycleBin");
                Logger.WriteLogFile(Utils.PrintEraseDate(tempInfo));

                Console.WriteLine(Utils.PrintDrivesStat());
                Logger.WriteLogFile(Utils.PrintDrivesStat());

                //Logs
                Console.Write("Eliminando logs antiguos...\t\t\t\t");
                Logger.WriteLogFile("Eliminando logs antiguos");
                var delLogs = Logger.CleanLogs();
                if (delLogs.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    Console.ForegroundColor = defaultColor;
                    foreach (var log in delLogs)
                        Logger.WriteLogFile(log);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("WARN");
                    Console.ForegroundColor = defaultColor;
                    Logger.WriteLogFile("No se encontraron logs para eliminar");
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine(e.Message);
                Logger.WriteLogFile(e.Message);
                Logger.WriteLogFile(e.StackTrace);
            }
            //Envio de mail.
            Report.SetFinalDiskInfo(Utils.GetDisks());
            SMTP.SendReport();
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
