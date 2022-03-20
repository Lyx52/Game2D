using OpenTK.Windowing.Common;

namespace Game.Input {
    public class KeyboardHandler {
        private bool[] inputArray;
        public KeyboardHandler() {
            this.inputArray = new bool[512];
        }
        public void OnKeyUp(KeyboardKeyEventArgs args) {
            this.inputArray[(int)args.Key] = false;
        }
        public void OnKeyDown(KeyboardKeyEventArgs args) {
            this.inputArray[(int)args.Key] = true;
        }
        public bool GetKey(int keyCode) {
            return this.inputArray[keyCode];
        }
        public int GetKeyI(int keyCode) {
            return this.inputArray[keyCode] ? 1 : 0;
        }
    }
}