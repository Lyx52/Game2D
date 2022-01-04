using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Game.Input;
using Game.Graphics;
using Game.Entity;
using Game.World;
using System;
using System.ComponentModel;

namespace Game.Core {
    public class Application : GameWindow {
        public static KeyboardHandler Keyboard { get; private set; }
        public static MouseHandler Mouse { get; private set; }
        public Renderer Renderer { get; }
        public GameWorld World { get; protected set; }
        public Application(string title, int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
            this.Size = new Vector2i(width, height);
            this.Title = title;
            this.UpdateFrequency = 30;

            // Bind keyboard handler to key events
            Keyboard = new KeyboardHandler();
            this.KeyUp += Keyboard.OnKeyUp;
            this.KeyDown += Keyboard.OnKeyDown;

            // Bind mouse handler to mouse events
            Mouse = new MouseHandler();
            this.MouseDown += Mouse.OnMouseDown;
            this.MouseUp += Mouse.OnMouseUp;
            this.MouseWheel += Mouse.OnMouseWheel;
            this.MouseMove += Mouse.OnMouseMove;

            // Create renderer
            this.Renderer = new Renderer(GameHandler.MAX_BUFFER_MEMORY, Vector2i.Divide(GameHandler.WindowSize, GameHandler.AspectRatio));
            this.Resize += this.Renderer.OnResize;

            Renderer.LoadTexture("grass", "./res/textures/grass.png");
            Renderer.LoadTexture("apple", "./res/textures/apple.png");
            Renderer.LoadTexture("spritesheet", "./res/textures/tile_spritesheet.png");

            // Display system info
            GameHandler.Logger.Info($"OS Version: {System.Environment.OSVersion}");
            GLHelper.DisplayGLInfo();
        }
        public void LoadWorld() {
            // Init world
            this.World = new GameWorld(0);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Renderer.StartScene(this.World.GetPlayer().KinematicBody.Position);
            this.World.Render(this.Renderer);
            Renderer.EndScene();
            base.OnRenderFrame(args);
            Context.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            this.World.Update(args.Time);
            this.UpdateTitle(args);
            base.OnUpdateFrame(args);
        }
        private void UpdateTitle(FrameEventArgs args) {
            this.Title = $"FPS: {Math.Round(1 / this.RenderTime, 2)}, UPS: {Math.Round(1 / this.UpdateTime, 2)}, Flushes: {this.Renderer.TotalFlushes}, Mem: {Math.Round((double)System.GC.GetTotalMemory(false) / (1000 * 1000), 2)}Mb, Vertices: {this.Renderer.PrevVertexCount}";
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            GameHandler.Logger.Debug("Closing window!");
            this.Renderer.Dispose();
            this.World.Dispose();
            base.OnClosing(e);
        }
    }
}