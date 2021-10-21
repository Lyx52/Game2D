using Game.Graphics;
using OpenTK.Mathematics;
namespace Game.Entity {
    public abstract class DrawableEntity : Entity {
        public Vector4 MaskColor { get; set; } 
        public Vector2[] TexCoords { get; set; }
        public Sprite Sprite { get; set; }
        public RenderLayer Layer { get; set; } 
        public DrawableEntity() {
            this.Sprite = new Sprite(Texture.WhiteTexture);
            this.MaskColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            this.TexCoords = Renderer.DefaultUVCoords;
            this.Layer = RenderLayer.BACKGROUND;
        }
        public abstract void Draw(Renderer renderer);

        public override string ToString() {
            return "DrawableEntity";
        }
        public Quad2D GetRenderQuad(PhysicalBody body) {
            return new Quad2D(body.Position, body.Size, this.Sprite.SpriteTexture, this.TexCoords, this.MaskColor, rotation:body.Rotation, layer:this.Layer);
        }
        public override string GetParrent()
        {
            return base.ToString();
        }
    }
}