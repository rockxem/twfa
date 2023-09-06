using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Permissions;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading;
using FileChecker.Models;

namespace FileChecker
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Timer timer = new Timer(FindAndScheduleJobs, null, TimeSpan.Zero, TimeSpan.FromHours(4));
            Console.WriteLine("Started");
            FindAndScheduleJobs(null);            
            Console.ReadLine();
        }

        private static void FindAndScheduleJobs(object state)
        {
            var currentDateTimeDetails = GetCurrentDateTimeDetails();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM FileStatusSchedule";
                    DataTable dataTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        adapter.Fill(dataTable);
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        FileStatusSchedule fileStatusSchedule = new FileStatusSchedule();
                        fileStatusSchedule.InterfaceName = row["InterfaceName"].ToString();
                        fileStatusSchedule.JobType = row["JobType"].ToString();
                        fileStatusSchedule.FilePath = row["FilePath"].ToString();
                        fileStatusSchedule.FileName = row["FileName"].ToString();
                        fileStatusSchedule.JobScheduleTimeHHMM = row["JobScheduleTimeHHMM"].ToString();

                        if (!string.IsNullOrEmpty(fileStatusSchedule.JobType))
                        {
                            if (fileStatusSchedule.JobType == "Daily")
                            {
                                int scheduleTime = CalculateScheduleTime(fileStatusSchedule.JobScheduleTimeHHMM);
                                ScheduledJobModel job = new ScheduledJobModel("Daily", scheduleTime);
                                //FindFileAndSaveLogs(fileStatusSchedule);
                                ScheduleJob(job, fileStatusSchedule);
                            }

                            if (fileStatusSchedule.JobType == "Weekly")
                            {
                                string[] WeeklyScheuleDaysOfWeek = row["WeeklyScheuleDaysOfWeek"].ToString().Split(',');
                                int scheduleTime = CalculateScheduleTime(fileStatusSchedule.JobScheduleTimeHHMM);
                                ScheduledJobModel job = new ScheduledJobModel("Weekly", scheduleTime);

                                if (WeeklyScheuleDaysOfWeek.Contains(currentDateTimeDetails.DayOfWeekNumber.ToString()))
                                {
                                    ScheduleJob(job, fileStatusSchedule);
                                    //FindFileAndSaveLogs(fileStatusSchedule);
                                }

                            }
                            if (fileStatusSchedule.JobType == "Monthly")
                            {
                                string[] MonthlyScheduleDaysOfMonth = row["MonthlyScheduleDaysOfMonth"].ToString().Split(',');
                                int scheduleTime = CalculateScheduleTime(fileStatusSchedule.JobScheduleTimeHHMM);
                                ScheduledJobModel job = new ScheduledJobModel("Monthly", scheduleTime);

                                if (MonthlyScheduleDaysOfMonth.Contains(currentDateTimeDetails.DayOfMonthNumber.ToString()))
                                {
                                    ScheduleJob(job, fileStatusSchedule);
                                    //FindFileAndSaveLogs(fileStatusSchedule);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static int CalculateScheduleTime(string jobTimeHHmmFromDB)
        {
            var currentDateTimeDetails = GetCurrentDateTimeDetails();
            int jobTimeInMinutes = ConvertHHMMToMinutes(jobTimeHHmmFromDB);
            int timeDiff = jobTimeInMinutes - currentDateTimeDetails.CurrentTimeMinutes;
            int scheduleTimeInMinutes;

            Random random = new Random();
            scheduleTimeInMinutes = timeDiff >= 0 ? timeDiff : random.Next(1,3);
            return scheduleTimeInMinutes;
        }

        private static List<FileMetadata> FindFileAndSaveLogs(FileStatusSchedule fileStatusSchedule)
        {
            string messageToLog = string.Empty;
            List<FileMetadata> fileMetadataList = new List<FileMetadata>();
            try
            {
                fileStatusSchedule.FilePath = @"D:\files"; //todo:comment afer testing

                string[] files = Directory.GetFiles(fileStatusSchedule.FilePath, fileStatusSchedule.FileName, SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    foreach (string filePath in files)
                    {
                        FileInfo file = new FileInfo(filePath);
                        FileSecurity fileSecurity = file.GetAccessControl();
                        AuthorizationRuleCollection ruleCollection = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));

                        FileMetadata metadata = new FileMetadata
                        {
                            FileName = file.Name,
                            CreationTime = file.CreationTime,
                            SizeBytes = file.Length
                        };
                        messageToLog = $"Found. File- {metadata.FileName} for Interface- {fileStatusSchedule.InterfaceName}, FileCreationDateTime- { metadata.CreationTime },  FileSizeInBytes- { metadata.SizeBytes}";
                        fileMetadataList.Add(metadata);
                    }
                }
                else
                {
                    messageToLog = $"Not_Found. File- {fileStatusSchedule.FileName} for Interface- {fileStatusSchedule.InterfaceName}";
                }
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileStatusLogs");
                LogToFile(logFilePath, messageToLog);
                Console.WriteLine(messageToLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex.Message);
            }
            return fileMetadataList;
        }

        static int ConvertHHMMToMinutes(string hhmm)
        {
            if (hhmm.Length != 4 || !int.TryParse(hhmm, out int timeValue))
            {
                throw new ArgumentException("Invalid time format. Use 'hhmm' format, e.g., '1430' for 14:30.");
            }

            int hours = timeValue / 100;
            int minutes = timeValue % 100;

            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                throw new ArgumentException("Invalid time values. Hours should be between 0-23 and minutes between 0-59.");
            }

            int totalMinutes = (hours * 60) + minutes;
            return totalMinutes;
        }
        private static void ScheduleJob(ScheduledJobModel job, FileStatusSchedule fileStatusSchedule)
        {
            TimeSpan timeUntilScheduledRun = TimeSpan.FromMinutes(job.ScheduledTime);

            Timer timer = new Timer(x => FindFileAndSaveLogs(fileStatusSchedule), null, timeUntilScheduledRun, Timeout.InfiniteTimeSpan);

            string messageToLog = $"Scheduled job for file- {fileStatusSchedule.FileName}, Interface - {fileStatusSchedule.InterfaceName}, at { DateTime.Now.AddMinutes(job.ScheduledTime) }  minutes";
            Console.WriteLine(messageToLog);
        }
        private static CurrentDateTimeDetails GetCurrentDateTimeDetails()
        {
            DateTime CurrentDateTime = DateTime.Now;
            int.TryParse(CurrentDateTime.ToString("HHmm"), out int currentTimeHHmm);
            DayOfWeek dayOfWeek = CurrentDateTime.DayOfWeek;
            int currentDateTimeMinutes = ConvertHHMMToMinutes(currentTimeHHmm.ToString());
            int dayOfWeekNumber = (int)dayOfWeek;
            int dayOfMonthNumber = CurrentDateTime.Day;

            return new CurrentDateTimeDetails
            {
                CurrentDateTime = CurrentDateTime,
                CurrentTimeHHmm = currentTimeHHmm,
                CurrentTimeMinutes = currentDateTimeMinutes,
                DayOfWeek = dayOfWeek,
                DayOfWeekNumber = dayOfWeekNumber,
                DayOfMonthNumber = dayOfMonthNumber
            };
        }
        private static void LogToFile(string FilePath, string message)
        {
            using (StreamWriter writer = File.AppendText(FilePath))
            {
                writer.WriteLine(message);
            }
        }
    }

}
