using OpenTK.Windowing.Common;

namespace Game.Input {
    public class KeyboardHandler {
        private static bool[] inputArray;
        public KeyboardHandler() {
            inputArray = new bool[512];
        }
        public void OnKeyUp(KeyboardKeyEventArgs args) {
            inputArray[(int)args.Key] = false;
        }
        public void OnKeyDown(KeyboardKeyEventArgs args) {
            inputArray[(int)args.Key] = true;
        }
        public static bool GetKey(int keyCode) {
            return inputArray[keyCode];
        }
        public static int GetKeyI(int keyCode) {
            return inputArray[keyCode] ? 1 : 0;
        }
    }
}