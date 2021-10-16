using OpenTK.Mathematics;
using Game.Graphics;

namespace Game.Entity {
    public class Player : DrawableEntity {
        public Player(float x, float y) : base() {
            this.AttachComponent<Vector2>(new Vector2(x, y), "position");
            this.AttachComponent<float>(0.0f, "rotation");
            this.AttachComponent<Vector2>(Vector2.One, "size");
        }
        public override void Draw(Renderer renderer) {
            renderer.DrawQuad(this.Position, this.Size, this.Texture, this.TexCoords, this.MaskColor, rotation:this.Rotation);
        }
        public float Rotation {
            get { return this.GetComponent<float>("rotation"); }
            set { this.SetComponent<float>("rotation", value); }
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
    }
}