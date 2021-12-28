using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Game.Input;
using Game.Graphics;
using Game.Entity;
using Game.Utils;
using System;

namespace Game.Core {
    public class Application : GameWindow {
        public KeyboardHandler Keyboard { get; }
        public MouseHandler Mouse { get; }
        public Renderer Renderer { get; }
        public EntityManager EntityHandler { get; }
        public Application(string title, int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
            GameHandler.Logger.Info($"OS Version: {System.Environment.OSVersion}");
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
            this.Renderer = new Renderer(GameHandler.MAX_BUFFER_MEMORY, Size.X, Size.Y);
            this.Resize += this.Renderer.OnResize;
            
            // // // Setup entity handler
            // this.EntityHandler = new EntityManager();
            // Random r = new Random();
            // // NOTE: Each entity is about 470-500 bytes
            // for (int i = 0; i < 10000; i++) {
            //     TestEntity entity = new TestEntity(i, (float)r.NextDouble());
            //     entity.Sprite.SpriteTexture = this.Renderer.DIRT_TEXTURE;
            //     this.EntityHandler.AddEntity(entity);
            // }
            
            // // Add player entity
            // Player player = new Player(0, 0);
            // player.Sprite.SpriteTexture = this.Renderer.APPLE_TEXTURE;
            // player.Controller.AttachKeyboardHandler(this.Keyboard);
            // player.Controller.AttachMouseHandler(this.Mouse);
            // // Vector2 projectionSize = this.Size / 8;
            // this.EntityHandler.AddEntity(player);
        }
        protected override void OnLoad()
        {
            base.OnLoad();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Renderer.StartScene();
            for (int i = 0; i < 2; i++)
                for (int r = 0; r < 2; r++)
                    Renderer.DispatchQuad(new Quad2D(new Vector2(i * 1.05f, r * 1.05f), Vector2.One, Renderer.spriteSheet.SpriteTexture, Renderer.spriteSheet.GetSubSprite(i, r), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), layer:RenderLayer.BACKGROUND));

            Renderer.EndScene();
            base.OnRenderFrame(args);
            Context.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }
        private void UpdateTitle(FrameEventArgs args) {
            this.Title = $"FPS: {Math.Round(1 / this.RenderTime, 2)}, UPS: {Math.Round(1 / this.UpdateTime, 2)}, Flushes: {this.Renderer.TotalFlushes}, Mem: {Math.Round((double)System.GC.GetTotalMemory(false) / (1000 * 1000), 2)}Mb";
        }
    }
}