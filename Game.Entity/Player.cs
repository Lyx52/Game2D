using OpenTK.Mathematics;
using Game.Graphics;
using Game.Input;
using Game.Utils;
using System;
namespace Game.Entity {
    public class Player : DrawableEntity {
        public Player(float x, float y) : base() {
            this.AttachComponent<Vector2>(new Vector2(x, y), "position");
            this.AttachComponent<float>(0.0f, "rotation");
            this.AttachComponent<float>(0.8f, "drag");
            this.AttachComponent<double>(1.0D, "acceleration");
            this.AttachComponent<Vector2>(Vector2.One, "size");
            this.AttachComponent<Vector2>(Vector2.Zero, "velocity");
            this.AttachComponent<EntityController>(new EntityController(this), "controller");
            this.AttachComponent<OrthoCamera>(new OrthoCamera(-1, 1, -1, 1, this.Position), "camera");
        }
        public override void Draw(Renderer renderer) {
            this.Camera.SetViewMatrix(renderer);
            renderer.DrawQuad(this.Position, this.Size, this.Texture, this.TexCoords, this.MaskColor, rotation:this.Rotation);
        }
        public EntityController Controller {
            get { return this.GetComponent<EntityController>("controller"); }
        }
        public double Acceleration {
            get { return this.GetComponent<double>("acceleration"); }
            set { this.SetComponent<double>("acceleration", value); }
        }
        public float Drag {
            get { return this.GetComponent<float>("drag"); }
            set { this.SetComponent<float>("drag", value); }
        }
        public float Rotation {
            get { return this.GetComponent<float>("rotation"); }
            set { this.SetComponent<float>("rotation", value); }
        }
        public Vector2 Velocity {
            get { return this.GetComponent<Vector2>("velocity"); }
            set { this.SetComponent<Vector2>("velocity", value); }
        }
        public float VelocityX {
            get { return this.Velocity.X; }
            set { this.SetComponent<Vector2>("velocity", new Vector2(value, this.Velocity.Y)); }
        }
        public float VelocityY {
            get { return this.Velocity.Y; }
            set { this.SetComponent<Vector2>("velocity", new Vector2(this.Velocity.X, value)); }
        }
        public Vector2 Size {
            get { return this.GetComponent<Vector2>("size"); }
            set { this.SetComponent<Vector2>("size", value); }
        }
        public float Width {
            get { return this.Size.X; }
            set { this.SetComponent<Vector2>("size", new Vector2(value, this.Size.Y)); }
        }
        public float Height {
            get { return this.Size.Y; }
            set { this.SetComponent<Vector2>("size", new Vector2(this.Size.X, value)); }
        }
        public ReadOnlySpan<Vector2> PositionSpan {
            get { return IOUtils.GetObjectSpan<Vector2>(this.Position); }
        }
        public Vector2 Position {
            get { return this.GetComponent<Vector2>("position"); }
            set { this.SetComponent<Vector2>("position", value); }
        }
        public float X {
            get { return this.Position.X; }
            set { this.SetComponent<Vector2>("position", new Vector2(value, this.Position.Y)); }
        }
        public float Y {
            get { return this.Position.Y; }
            set { this.SetComponent<Vector2>("position", new Vector2(this.Position.X, value)); }
        }
        public OrthoCamera Camera {
            get { return this.GetComponent<OrthoCamera>("camera"); }
        }
        public Vector2 CameraPosition {
            get { return this.Camera.Position;}
            set { this.GetComponent<OrthoCamera>("camera").SetPosition(value); }
        }
        public void AttachKeyboardHandler(KeyboardHandler handler) {
            this.GetComponent<EntityController>("controller").AttachKeyboardHandler(handler);
        }
        public override void Update(double dt) {
            this.Velocity += Vector2.Multiply(this.Controller.GetDirectional(), (float)(this.Acceleration * dt));
            this.Position += this.Velocity;
            this.CameraPosition += this.Velocity;
            this.Velocity *= this.Drag;
        }
        public override string ToString() {
            return "Player";
        }
    }
}