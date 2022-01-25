using Game.Utils;
using Game.Graphics;
using Game.Core;
using OpenTK.Mathematics;

namespace Game
{
    class GameHandler
    {
        public static Renderer Renderer;
        public static int MAX_BUFFER_MEMORY = 32768;
        
        // These values need to be loaded from settings
        public static Vector2i AspectRatio = new Vector2i(4, 3);
        public static Vector2i WindowSize = new Vector2i(800, 600);
        static void Main(string[] args)
        {
            using (Application game = new Application("SimpleGame2D", WindowSize.X, WindowSize.Y)) {
                Renderer = game.Renderer;
                game.LoadWorld();
                game.Run(); 
            }
            Logger.Close();
        }
    }
}
