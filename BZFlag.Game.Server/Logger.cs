using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace BZFlag.Game.Host
{
    public static class Logger
    {
        public static bool ShowDateTime = false;
        public static FileInfo LogFile = null;

        private static List<string> PendingLogUpdates = new List<string>();
        private static Thread LogWriter = null;

        public static int LogLevel = 1;

        public class LogEventArgs : EventArgs
        {
            public int Level = 0;
            public string Timestamp = string.Empty;
            public string Text = string.Empty;
        }

        public static event EventHandler<LogEventArgs> LineLogged;

        public static void SetLogFilePath(string filePath)
        {
            if (filePath != string.Empty && File.Exists(filePath))
                LogFile = new FileInfo(filePath);
            else
                LogFile = null;
        }

        public static void Log(int level, string data)
        {
            if (level > LogLevel)
                return;

            LogEventArgs args = new LogEventArgs();
            args.Timestamp = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString();
            args.Text = data;
            args.Level = level;

            LineLogged?.Invoke(null, args);

            string line = "Level " + level.ToString() + " ";
            if (ShowDateTime)
                line += args.Timestamp  + " ";
            line += data;

            Console.WriteLine(line);

            if (LogFile != null)
                PushLogFileUpdate(line);
        }

        private static void PushLogFileUpdate(string text)
        {
            lock (PendingLogUpdates)
                PendingLogUpdates.Add(text);


            if (LogWriter == null)
            {
                LogWriter = new Thread(new ThreadStart(WriteLog));
                LogWriter.Start();
            }
        }

        private static void WriteLog()
        {
            if (LogFile != null)
            {
                var fs = new FileStream(LogFile.FullName, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);

                bool done = false;
                while (!done)
                {
                    string line = string.Empty;
                    lock (PendingLogUpdates)
                    {
                        if (PendingLogUpdates.Count == 0)
                            done = true;
                        else
                        {
                            line = PendingLogUpdates[0];
                            PendingLogUpdates.RemoveAt(0);
                        }
                    }

                    if (line != string.Empty)
                    {
                        sw.WriteLine(line);
                    }
                }

                sw.Flush();
                sw.Close();
                fs.Close();
            }

            LogWriter = null;
        }

        public static void Log0(string data)
        {
            Log(0, data);
        }

        public static void Log1(string data)
        {
            Log(1, data);
        }
        public static void Log2(string data)
        {
            Log(2, data);
        }

        public static void Log3(string data)
        {
            Log(3, data);
        }

        public static void Log4(string data)
        {
            Log(4, data);
        }
    }
}
