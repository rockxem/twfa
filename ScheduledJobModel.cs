using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileChecker.Models
{
    class ScheduledJobModel
    {
        public string Name { get; }
        public int ScheduledTime { get; }

        public ScheduledJobModel(string name, int scheduledTime)
        {
            Name = name;
            ScheduledTime = scheduledTime;
        }
    }
}
