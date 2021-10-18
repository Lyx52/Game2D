using Game.Graphics;
using OpenTK.Mathematics;

namespace Game.Entity {
    public class TestEntity : DrawableEntity {
        public TestEntity(float x, float y) {
            this.AttachComponent<Vector2>(new Vector2(x, y), "position");
            this.AttachComponent<Vector2>(Vector2.One, "size");
            this.AttachComponent<float>(0, "rotation");
        }
        public override void Update(double dt) {
        
        }
        public override void Draw(Renderer renderer)
        {
            renderer.DrawQuad(this.Position, this.Size, this.Texture, this.TexCoords, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), rotation:this.Rotation);
            this.Rotation += 1.0f;
        }
         public Vector2 Position {
            get { return this.GetComponent<Vector2>("position"); }
            set { this.SetComponent<Vector2>("position", value); }
        }
        public Vector2 Size {
            get { return this.GetComponent<Vector2>("size"); }
            set { this.SetComponent<Vector2>("size", value); }
        }
        public float Rotation {
            get { return this.GetComponent<float>("rotation"); }
            set { this.SetComponent<float>("rotation", value); }
        }
    }
}