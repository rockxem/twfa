using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileChecker.Models
{
    class CurrentDateTimeDetails
    {
        public DateTime CurrentDateTime { get; set; }
        public int CurrentTimeHHmm { get; set; }
        public int CurrentTimeMinutes { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int DayOfWeekNumber { get; set; }
        public int DayOfMonthNumber { get; set; }

    }
}
