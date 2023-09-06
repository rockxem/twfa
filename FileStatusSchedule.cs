using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileChecker.Models
{
    public class FileStatusSchedule
    {
        public int InterfaceId { get; set; }
        public string InterfaceName { get; set; }
        public string JobType { get; set; }
        public string JobScheduleTimeHHMM { get; set; }
        public string WeeklyScheuleDaysOfWeek { get; set; }
        public string MonthlyScheduleDaysOfMonth { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
