using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileChecker.Models
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public DateTime CreationTime { get; set; }
        public long SizeBytes { get; set; }
    }
}