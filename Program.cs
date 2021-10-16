using Game.Utils;

namespace Game
{
    class GameHandler
    {
        private static Logger MainLogger;
        private static Profiler MainProfiler;

        private static Core.Game game;

        public static int MAX_BUFFER_MEMORY = 32768;
        
        // These values need to be loaded from settings
        private static int WIDTH = 800;
        private static int HEIGHT = 600;
        static void Main(string[] args)
        {
            MainLogger = new Logger();
            MainProfiler = new Profiler();

            using (game = new Core.Game("SimpleGame2D", WIDTH, HEIGHT)) {
                game.Run();
            }
            MainLogger.Close();
        }
        public static Core.Game CoreGame {
            get {
                return game;
            }
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
