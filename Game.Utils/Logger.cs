#define FILE_LOGGING
using System;
#if FILE_LOGGING
using System.IO;
#endif

namespace Game.Utils {
    public static class Logger {
        public enum LogLevel {
            INFO,
            WARN,
            DEBUG,
            ERROR,
            CRITICAL
        }
        private static int _LoggingLevel;
        
        #if FILE_LOGGING
        private static StreamWriter _LogWriter;
        #endif
        static Logger() {
            _LoggingLevel = 0;
            _LogWriter = IOUtils.GetLogWriter();
        }
        public static void Close() {
            #if FILE_LOGGING
            _LogWriter.WriteLine($"Log::End - {DateTime.Now}");
            _LogWriter.Flush();
            _LogWriter.Close();
            #endif
        }
        public static int LoggingLevel {
            get { return _LoggingLevel; }
            set { _LoggingLevel = Math.Min(Math.Max(value, 0), 4); }
        }

        public static void Log(string message, LogLevel severity) {
            string log_message = $"Log::{severity}({DateTime.Now}) - {message}";
            if (LoggingLevel <= ((int)severity)) {
                Console.WriteLine(log_message);
                #if FILE_LOGGING
                _LogWriter.WriteLine(log_message);
                _LogWriter.Flush();
                #endif
            }
        }
        public static void Info(string message) {
            Log(message, LogLevel.INFO);
        }
        public static void Warn(string message) {
            Log(message, LogLevel.WARN);
        }
        public static void Debug(string message) {
            Log(message, LogLevel.DEBUG);
        }
        public static void Error(string message) {
            Log(message, LogLevel.ERROR);
        }
        public static void Critical(string message) {
            Log(message, LogLevel.CRITICAL);
            Environment.Exit(1);
        }
        public static void Assert(bool assertion, string errorMessage) {
            if (!assertion)
                Critical($"ASSERTION FAILED: {errorMessage}");
        }
    }
}