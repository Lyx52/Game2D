using Game.Utils;

namespace Game
{
    class GameHandler
    {
        private static Logger MainLogger;
        private static Profiler MainProfiler;
        static void Main(string[] args)
        {

            MainLogger = new Logger();
            MainProfiler = new Profiler();

            using (Core.Game game = new Core.Game("SimpleGame2D", 800, 600)) {
                game.Run();
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
