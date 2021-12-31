using Game.Utils;
using Game.Graphics;
using Game.Core;
using OpenTK.Mathematics;

namespace Game
{
    class GameHandler
    {
        private static Logger MainLogger;
        private static Profiler MainProfiler;
        private static Renderer MainRenderer;

        public static int MAX_BUFFER_MEMORY = 32768;
        
        // These values need to be loaded from settings
        public static Vector2i AspectRatio = new Vector2i(4, 3);
        public static Vector2i WindowSize = new Vector2i(800, 600);
        // public static Vector2i AspectRatio = new Vector2i(16, 9);
        // public static Vector2i WindowSize = new Vector2i(1920, 1080);
        static void Main(string[] args)
        {
            MainLogger = new Logger();
            MainProfiler = new Profiler();

            using (Application game = new Application("SimpleGame2D", WindowSize.X, WindowSize.Y)) {
                MainRenderer = game.Renderer;
                game.Run(); 
            }
            MainLogger.Close();
        }
        public static Logger Logger {
            get { return MainLogger; }
        }
        public static Profiler Profiler {
            get { return MainProfiler; }
        }
        public static Renderer Renderer {
            get { return MainRenderer; }
        }
    }
}
