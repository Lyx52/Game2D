using System;
#if FILE_LOGGING
using System.IO;
#endif

namespace Game.Utils {
    public class Logger {
        public enum LogLevel {
            INFO,
            WARN,
            DEBUG,
            ERROR,
            CRITICAL
        }
        private int LoggingLevel = 0;
        
        #if FILE_LOGGING
        private StreamWriter LogWriter = FileUtils.GetLogWriter();
        #endif

        public void Close() {
            #if FILE_LOGGING
            LogWriter.WriteLine($"Log::End - {DateTime.Now}");
            LogWriter.Flush();
            LogWriter.Close();
            #endif
        }

        public void Log(string message, LogLevel severity) {
            string log_message = $"Log::{severity}({DateTime.Now}) - {message}";
            if (this.LoggingLevel <= ((int)severity)) {
                Console.WriteLine(log_message);
                #if FILE_LOGGING
                LogWriter.WriteLine(log_message);
                LogWriter.Flush();
                #endif
            }
        }
        public void Info(string message) {
            this.Log(message, LogLevel.INFO);
        }
        public void Warn(string message) {
            this.Log(message, LogLevel.WARN);
        }
        public void Debug(string message) {
            this.Log(message, LogLevel.DEBUG);
        }
        public void Error(string message) {
            this.Log(message, LogLevel.ERROR);
        }
        public void Critical(string message) {
            this.Log(message, LogLevel.CRITICAL);
            Environment.Exit(1);
        }
    }
}