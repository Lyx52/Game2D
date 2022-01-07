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
        public static FileStream OpenReadWriteStream(string filePath) {
            return File.Open(filePath, FileMode.OpenOrCreate);
        }
        public static FileStream OpenReadStream(string filePath) {
            if (File.Exists(filePath)) {
                return File.OpenRead(filePath); 
            } else {
                GameHandler.Logger.Critical($"File {filePath} does not exist!");
                return null;
            } 
        }
        public static byte[] ReadFile(string filePath) {
            using (FileStream fs = OpenReadStream(filePath)) {
                byte[] data = new byte[fs.Length];
                try {
                    fs.Read(data, 0, data.Length);
                } catch (IOException e) {
                    GameHandler.Logger.Error($"Error while reading the file!\n{e}");
                }
                return data;
            }
        }
        public static StreamWriter GetLogWriter(string logFolder="logs", string logFilePrefix="gamelog") {
            string absolutePath = Path.Combine(GetCWD(), logFolder);
            if (!Directory.Exists(absolutePath)) {
                Directory.CreateDirectory(absolutePath);
            }
            string logPath = Path.Combine(absolutePath, $"{logFilePrefix}_{StringUtils.GetShortDate(seperator:"")}_{StringUtils.GetShortTime(seperator:"")}.log");
            
            StreamWriter writer = new StreamWriter(logPath);
            writer.WriteLine($"Log::Start - {DateTime.Now}");
            writer.Flush();
            return writer;
        }
        public static GCHandle GetObjHandle(object allocatedObject) {
            return GCHandle.Alloc(allocatedObject, GCHandleType.Pinned);
        }
        public static byte[] StringAsBytes(string text) {
            // Function that turns string into byte array + appends 0xFF at the end 
            byte[] data = new byte[text.Length + 1];
            Array.Copy(Encoding.UTF8.GetBytes(text), data, text.Length);
            data[text.Length] = 0xFF; // Last byte is terminator 0xFF
            return data;  
        }
        public static string ReadString(FileStream stream) {
            // Reads string from stream, until it reaches 0xFF terminator
            string output = "";
            int data = stream.ReadByte();
            do {
                output += (char)data;
                data = stream.ReadByte();
            } while (data != 0xFF); // Terminator not reached

            return output;
        }
    }
    public static class StringUtils {
        
        public static string GetShortTime(string seperator=":") {
            return $"{DateTime.Now.Hour}{seperator}{DateTime.Now.Minute}{seperator}{DateTime.Now.Second}";
        }

        public static string GetShortDate(string seperator=".") {
            return $"{DateTime.Now.Year}{seperator}{DateTime.Now.Month}{seperator}{DateTime.Now.Day}";
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