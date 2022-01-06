using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Game.Utils {
    public static class IOUtils {
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
                GameHandler.Logger.Critical($"File {filePath} does not exist!");
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
        public static GCHandle GetObjHandle(object allocatedObject) {
            return GCHandle.Alloc(allocatedObject, GCHandleType.Pinned);
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
    public static class ArrayUtils {
        public static int IndexOf<T>(object[] arr, object obj) {
            for (int i = 0; i < arr.Length; i++) {
                if (((T)arr[i]).Equals((T)obj)) {
                    return i;
                }
            }
            return -1;
        }
    }
    public static class MathUtils {
        public static double ToRadians(double value) {
            return (Math.PI / 180) * value;
        }
        public static float Rad2Deg {
            get { return (float)(360 / (Math.PI * 2));}
        }
        public static float Deg2Rad {
            get { return (float)((Math.PI * 2) / 360);}
        }
        public static float LookAt(Vector2 start, Vector2 target) {
            Vector2 diff = (start - target).Normalized();
            return (MathF.Atan2(diff.X, diff.Y) * MathUtils.Rad2Deg);
        }
        public static Vector2 Rotate(Vector2 vectorIn, float angle) {
            return vectorIn * Matrix2.CreateRotation(angle);
        }
        public static int ToInt(bool value) {
            return value ? 1 : 0;
        }
        public static Matrix4 CreateTransform(Vector2 position, Vector2 size, float rotation) {
            Matrix4 _translation = Matrix4.CreateTranslation(position.X, position.Y, 0);
            Matrix4 _rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians((float)rotation));
            Matrix4 _scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
            return Matrix4.Mult(Matrix4.Mult(_scale, _rotation), _translation);
        }
    }
}