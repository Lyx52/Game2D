using Game2D.Core;
using Game2D.Utils;

namespace Game2D
{
    class GameHandler
    {
        private static Logger MainLogger;
        static void Main(string[] args)
        {

            MainLogger = new Logger();

            using (Game game = new Game("SimpleGame2D", 800, 600)) {
                game.Run();
            }
        }
        public static Logger Logger {
            get {
                return MainLogger;
            }
        }
    }
}
