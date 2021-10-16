using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Game.Input;
using Game.Graphics;
using System;

namespace Game.Core {
    public class Game : GameWindow {
        private KeyboardHandler Keyboard;
        private MouseHandler Mouse;
        private Renderer Renderer;
        public Game(string title, int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
            this.Size = new Vector2i(width, height);
            this.Title = title;
            this.UpdateFrequency = 20;

            // Bind keyboard handler to key events
            this.Keyboard = new KeyboardHandler();
            this.KeyUp += this.Keyboard.OnKeyUp;
            this.KeyDown += this.Keyboard.OnKeyDown;

            // Bind mouse handler to mouse events
            this.Mouse = new MouseHandler();
            this.MouseDown += this.Mouse.OnMouseDown;
            this.MouseUp += this.Mouse.OnMouseUp;
            this.MouseWheel += this.Mouse.OnMouseWheel;
            this.MouseMove += this.Mouse.OnMouseMove;
            
            // Bind renderer to window frame render events
            this.Renderer = new Renderer(GameHandler.MAX_BUFFER_MEMORY, Size.X, Size.Y);
            this.RenderFrame += this.Renderer.OnRenderFrame;
            this.Resize += this.Renderer.OnResize;
            this.Renderer.OnEndScene += this.OnEndScene;

            // Bind any other events
            this.UpdateFrame += this.UpdateTitle;
        }
        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);
        }
        
        private void OnEndScene(FrameEventArgs args) {
            Context.SwapBuffers();
        }
        private void UpdateTitle(FrameEventArgs args) {
            this.Title = $"FPS: {Math.Round(1 / this.RenderTime, 2)}, UPS: {Math.Round(1 / this.UpdateTime, 2)}, Flushes: {this.Renderer.TotalFlushes}";
        }
    }
}