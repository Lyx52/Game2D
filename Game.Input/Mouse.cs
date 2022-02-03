using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;
namespace Game.Input {
    public enum MouseButton {
        LEFT,
        RIGHT,
        MIDDLE
    }
    public class MouseHandler {
        private static bool[] mouseButtons;
        private static Vector2 currentPosition;
        private static Vector2 deltaPosition;
        public MouseHandler() {
            mouseButtons = new bool[8];
            currentPosition = Vector2.Zero;
            deltaPosition = Vector2.Zero;
        }
        public static event Action<MouseWheelEventArgs> MouseWheel;
        public void OnMouseWheel(MouseWheelEventArgs args) {
            MouseWheel?.Invoke(args);
        }
        public void OnMouseUp(MouseButtonEventArgs args) {
            mouseButtons[(int)args.Button] = false;
        }
        public void OnMouseDown(MouseButtonEventArgs args) {
            mouseButtons[(int)args.Button] = true;
        }
        public void OnMouseMove(MouseMoveEventArgs args) {
            deltaPosition = args.Delta;
            currentPosition.X = args.X;
            currentPosition.Y = args.Y;
        }
        public static bool GetButton(MouseButton button) {
            return mouseButtons[(int)button];
        }
        public static Vector2 GetPosition() {
            return currentPosition;
        }
        public static Vector2 GetDeltaPosition() {
            return deltaPosition;
        }
    }
}