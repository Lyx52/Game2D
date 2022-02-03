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
        private static LogLevel _LoggingLevel;
        
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
        public static LogLevel LoggingLevel {
            get { return _LoggingLevel; }
            set { _LoggingLevel = (LogLevel)Math.Min(Math.Max((int)value, 0), 4); }
        }

        public static void Log(string message, LogLevel severity) {
            string log_message = $"Log::{severity}({DateTime.Now}) - {message}";
            if ((int)LoggingLevel <= (int)severity) {
                Console.WriteLine(log_message);
                #if FILE_LOGGING
                _LogWriter.WriteLine(log_message);
                _LogWriter.Flush();
                #endif
            }
        }
        public static void Info(object obj) {
            Info(obj.ToString());
        }
        public static void Info(string message) {
            Log(message, LogLevel.INFO);
        }
        public static void Warn(object obj) {
            Warn(obj.ToString());
        }
        public static void Warn(string message) {
            Log(message, LogLevel.WARN);
        }
        public static void Debug(object obj) {
            Debug(obj.ToString());
        }
        public static void Debug(string message) {
            Log(message, LogLevel.DEBUG);
        }
        public static void Error(object obj) {
            Error(obj.ToString());
        }
        public static void Error(string message) {
            Log(message, LogLevel.ERROR);
        }
        public static void Critical(object obj) {
            Critical(obj.ToString());
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