using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_Dota2
{
    public enum Logging_Level
    {
        None,
        Info,
        Warning,
        Error
    }

    public class Logger
    {
        private bool retrieved_unique_logfile = false;
        private string logfile = "log.txt";
        private string logdir = "logs/";

        public string GetPath()
        {
            if (!retrieved_unique_logfile)
                logfile = System.IO.Path.Combine(logdir, System.DateTime.Now.ToString("yyyy_dd_MM") + ".log");

            if (!System.IO.File.Exists(logdir))
                System.IO.Directory.CreateDirectory(logdir);

            return logfile;
        }

        public void LogLine(string message, Logging_Level level = Logging_Level.None, bool timestamp = true)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(GetPath());
            try
            {
                string logLine = (level != Logging_Level.None ? "[" + LevelToString(level) + "] " : "") + System.String.Format("{0:G}: {1}.", System.DateTime.Now, message);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        private string LevelToString(Logging_Level level)
        {
            switch (level)
            {
                case (Logging_Level.Info):
                    return "INFO";
                case (Logging_Level.Warning):
                    return "WARNING";
                case (Logging_Level.Error):
                    return "ERROR";
                default:
                    return "";
            }
        }
    }
}
