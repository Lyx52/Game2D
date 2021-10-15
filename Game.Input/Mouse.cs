using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;
namespace Game2D.Input {
    public enum MouseButton {
        LEFT,
        RIGHT,
        MIDDLE
    }
    public class MouseHandler {
        private bool[] mouseButtons;
        private Vector2 currentPosition;
        private Vector2 deltaPosition;
        public MouseHandler() {
            this.mouseButtons = new bool[8];
            this.currentPosition = Vector2.Zero;
            this.deltaPosition = Vector2.Zero;
        }
        public event Action<MouseWheelEventArgs> MouseWheel;
        public void OnMouseWheel(MouseWheelEventArgs args) {
            MouseWheel?.Invoke(args);
        }
        public void OnMouseUp(MouseButtonEventArgs args) {
            this.mouseButtons[(int)args.Button] = false;
        }
        public void OnMouseDown(MouseButtonEventArgs args) {
            this.mouseButtons[(int)args.Button] = true;
        }
        public void OnMouseMove(MouseMoveEventArgs args) {
            this.deltaPosition = args.Delta;
            this.currentPosition.X = args.X;
            this.currentPosition.Y = args.Y;
        }
        public bool GetButton(MouseButton button) {
            return this.mouseButtons[(int)button];
        }
        public Vector2 GetPosition() {
            return this.currentPosition;
        }
        public Vector2 GetDeltaPosition() {
            return this.deltaPosition;
        }
    }
}