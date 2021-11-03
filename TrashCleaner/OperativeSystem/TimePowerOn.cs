using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashCleaner.OperativeSystem
{
    public class TimePowerOn
    {
        public static string GetTimePowerOn()
        {
            int milisec = Environment.TickCount & int.MaxValue;
            int seconds = milisec / 1000;
                milisec = milisec % 1000;
            int minutes = seconds / 60;
                seconds = seconds % 60;
            int hours = minutes / 60;
                minutes = minutes % 60;
            int days = hours / 24;
                hours = hours % 24;

            string timeEmplased = $"{days} día(s) y {hours}:{minutes}:{seconds}.{milisec}";

            return timeEmplased;
        }
    }
}
