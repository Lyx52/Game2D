using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Game.Input;
using Game.Graphics;
using Game.Entity;
using System;
using System.ComponentModel;

namespace Game.Core {
    public class Application : GameWindow {
        public KeyboardHandler Keyboard { get; }
        public MouseHandler Mouse { get; }
        public Renderer Renderer { get; }
        public EntityManager EntityHandler { get; }
        public SpriteSheet spriteSheet;
        public Application(string title, int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
            this.Size = new Vector2i(width, height);
            this.Title = title;
            this.UpdateFrequency = 30;

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

            // Create renderer
            this.Renderer = new Renderer(GameHandler.MAX_BUFFER_MEMORY, Vector2i.Divide(GameHandler.WindowSize, GameHandler.AspectRatio));
            this.Resize += this.Renderer.OnResize;
            Renderer.LoadTexture("grass", "./res/textures/grass.png");
            Renderer.LoadTexture("apple", "./res/textures/apple.png");
            this.spriteSheet = new SpriteSheet(Renderer.GetTexture("grass"), 2, 2);
            // // // Setup entity handler
            this.EntityHandler = new EntityManager();
            Random r = new Random();
            
            // Add player entity
            Player player = new Player(0, 0);
            player.Sprite.SpriteTexture = Renderer.GetTexture("apple");
            player.Controller.AttachKeyboardHandler(this.Keyboard);
            player.Controller.AttachMouseHandler(this.Mouse);
            // // Vector2 projectionSize = this.Size / 8;
            this.EntityHandler.AddEntity(player);
            
            // NOTE: Each entity is about 470-500 bytes
            for (int i = 0; i < 100; i++) {
                TestEntity entity = new TestEntity(i, (float)r.NextDouble());
                entity.Sprite.SpriteTexture = this.Renderer.GetTexture("grass");
                this.EntityHandler.AddEntity(entity);
            }
            
            // Display system info
            GameHandler.Logger.Info($"OS Version: {System.Environment.OSVersion}");
            GLHelper.DisplayGLInfo();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Renderer.StartScene(this.EntityHandler.GetPlayer().KinematicBody.Position);
            for (int i = 0; i < 2; i++)
                for (int r = 0; r < 2; r++)
                    Renderer.DispatchQuad(new DrawQuad2D(new Vector2(i * 16.05f, r * 16.05f), new Vector2(16, 16), this.spriteSheet.SpriteTexture, this.spriteSheet.GetSubSprite(i, r), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), layer:RenderLayer.BACKGROUND));
            this.EntityHandler.Render(this.Renderer);
            Renderer.EndScene();
            base.OnRenderFrame(args);
            Context.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            this.EntityHandler.Update(args.Time);
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
            this.EntityHandler.Dispose();
            base.OnClosing(e);
        }
    }
}