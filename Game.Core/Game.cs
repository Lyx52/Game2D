using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Game.Input;
using Game.Graphics;
using Game.Entity;

using System;

namespace Game.Core {
    public class Game : GameWindow {
        public KeyboardHandler Keyboard { get; }
        public MouseHandler Mouse { get; }
        public Renderer Renderer { get; }
        public EntityManager EntityHandler { get; }
        public Game(string title, int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
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
            
            // Bind renderer to window frame render events
            this.Renderer = new Renderer(GameHandler.MAX_BUFFER_MEMORY, Size.X, Size.Y);
            this.RenderFrame += this.Renderer.OnRenderFrame;
            this.Resize += this.Renderer.OnResize;
            this.Renderer.OnEndScene += this.SwapBuffers;
            
            // Setup entity handler
            this.EntityHandler = new EntityManager();

            TestEntity entity = new TestEntity(0, 0);
            entity.Texture = this.Renderer.DIRT_TEXTURE;
            this.EntityHandler.AddEntity(entity);
            
            // Add player entity
            Player player = new Player(0, 0);
            player.Texture = this.Renderer.APPLE_TEXTURE;
            player.AttachKeyboardHandler(this.Keyboard);
            this.EntityHandler.AddEntity(player);
            
            // Attach renderer to entity handler
            this.Renderer.OnRender += this.EntityHandler.OnRender;
            
            // Bind any other events
            this.UpdateFrame += this.UpdateTitle; 
        }
        private void SwapBuffers(FrameEventArgs args) {
            Context.SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            this.EntityHandler.OnUpdate(args.Time);
            base.OnUpdateFrame(args);
        }
        private void UpdateTitle(FrameEventArgs args) {
            this.Title = $"FPS: {Math.Round(1 / this.RenderTime, 2)}, UPS: {Math.Round(1 / this.UpdateTime, 2)}, Flushes: {this.Renderer.TotalFlushes}";
        }
    }
}