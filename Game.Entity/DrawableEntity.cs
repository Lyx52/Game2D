using Game.Graphics;
using OpenTK.Mathematics;
namespace Game.Entity {
    public abstract class DrawableEntity : Entity {

        public DrawableEntity() {
            this.AttachComponent<Texture>(Texture.WhiteTexture, "texture");
            this.AttachComponent<Vector4>(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), "maskColor");
            this.AttachComponent<Vector2[]>(Renderer.GetDefaultUVCoords(), "texCoords");
        }
        public Texture Texture {
            get { return this.GetComponent<Texture>("texture"); }
            set {this.SetComponent<Texture>("texture", value); }
        }
        public Vector4 MaskColor {
            get { return this.GetComponent<Vector4>("maskColor"); }
            set {this.SetComponent<Vector4>("maskColor", value); }
        }
        public Vector2[] TexCoords {
            get { return this.GetComponent<Vector2[]>("texCoords"); }
            set { this.SetComponent<Vector2[]>("texCoords", value); }
        }
        public abstract void Draw(Renderer renderer);
    }
}