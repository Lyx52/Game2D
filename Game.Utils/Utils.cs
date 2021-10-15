using System;
using System.IO;
using System.Text;

namespace Game.Utils {
    public static class FileUtils {
        public static string ReadTextFile(string filePath) {
            UTF8Encoding decoder = new UTF8Encoding(true);
            try {
                return decoder.GetString(ReadFile(filePath));
            } catch(ArgumentNullException e) {
                GameHandler.Logger.Error($"Error while converting to string!\n{e}");
            }
            return "";
        }
        public static string GetCWD() {
            return Directory.GetCurrentDirectory();
        }
        public static FileStream GetFileStream(string filePath) {
            if (File.Exists(filePath)) {
                return File.OpenRead(filePath); 
            } else {
                GameHandler.Logger.Error($"File {filePath} does not exist!");
                return null;
            } 
        }
        public static byte[] ReadFile(string filePath) {
            using (FileStream fs = GetFileStream(filePath)) {
                byte[] data = new byte[fs.Length];
                try {
                    fs.Read(data, 0, data.Length);
                } catch (IOException e) {
                    GameHandler.Logger.Error($"Error while reading the file!\n{e}");
                }
                return data;
            }
        }
        public static StreamWriter GetLogWriter(string logPath = "./logs/") {
            if (!Directory.Exists(logPath)) {
                Console.WriteLine($"Directory: {Directory.Exists(logPath)}");
                Directory.CreateDirectory(logPath);
            }
            StreamWriter writer = new StreamWriter(Path.Combine(logPath, $"gamelog_{StringUtils.GetShortTime()}_{StringUtils.GetShortDate()}.log"));
            writer.WriteLine($"Log::Start - {DateTime.Now}");
            writer.Flush();
            return writer;
        }

    }
    public static class StringUtils {
        
        public static string GetShortTime() {
            return $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
        }

        public static string GetShortDate() {
            return $"{DateTime.Now.ToShortDateString().Replace('/', '.')}";
        }

        public static string GetBytesAsString(byte[] array) {
            string output = "";
            foreach (byte value in array) {
                output += $"{value} ";
            }
            return output;
        }
    }
    public static class MathUtils {
        public static double ToRadians(double value) {
            return (Math.PI / 180) * value;
        }
        public static int ToInt(bool value) {
            return value ? 1 : 0;
        }
    }
}