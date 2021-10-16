using Game.Utils;
using Game.Graphics;

namespace Game
{
    class GameHandler
    {
        private static Logger MainLogger;
        private static Profiler MainProfiler;

        public static int MAX_BUFFER_MEMORY = 16384;
        
        // These values need to be loaded from settings
        private static int WIDTH = 800;
        private static int HEIGHT = 600;
        static void Main(string[] args)
        {
            MainLogger = new Logger();
            MainProfiler = new Profiler();

            using (Core.Game game = new Core.Game("SimpleGame2D", WIDTH, HEIGHT)) {
                game.Run();
            }
            MainLogger.Close();
        }
        public static Logger Logger {
            get {
                return MainLogger;
            }
        }
        public static Profiler Profiler {
            get {
                return MainProfiler;
            }
        }
    }
}
