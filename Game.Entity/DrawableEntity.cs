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
        public DrawQuad2D GetRenderQuad(Vector2 position, Vector2 size, float rotation) {
            return new DrawQuad2D(position, size, this.Sprite.SpriteTexture, this.Sprite.GetTexCoords(), this.MaskColor, rotation:rotation, layer:this.Layer);
        }
        public DrawQuad2D GetRenderQuad(PhysicalBody body) {
            return this.GetRenderQuad(body.Position, body.Size, body.Rotation);
        }
        public override string GetParrent()
        {
            return base.ToString();
        }
    }
}