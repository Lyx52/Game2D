using OpenTK.Windowing.Common;

namespace Game.Input {
    public class KeyboardHandler {
        private bool[] inputArray;
        public KeyboardHandler() {
            this.inputArray = new bool[256];
        }
        public void OnKeyUp(KeyboardKeyEventArgs args) {
            this.inputArray[args.ScanCode] = false;
        }
        public void OnKeyDown(KeyboardKeyEventArgs args) {
            this.inputArray[args.ScanCode] = true;
        }
        public bool GetKey(int keyCode) {
            return this.inputArray[keyCode];
        }
        public int GetKeyI(int keyCode) {
            return this.inputArray[keyCode] ? 1 : 0;
        }
    }
}