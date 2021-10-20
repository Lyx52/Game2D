using OpenTK.Mathematics;

namespace Game.Graphics {
    public enum SpriteType {
        SPRITE,
        SPRITE_SHEET,
        ANIMATED_SPRITE
    }
    public class Sprite {
        private Texture spriteTexture;
        public SpriteType Type { get; set; }
        public Vector2i TextureSize { get; set; }
        public int TextureWidth { get { return this.spriteTexture.Width; } }
        public int TextureHeight { get { return this.spriteTexture.Height; } }
        public Sprite(Texture texture) {
            this.spriteTexture = texture;
            this.Type = SpriteType.SPRITE;
            this.TextureSize = new Vector2i(texture.Width, texture.Height);
        }
        public virtual Texture GetTexture() {
            return this.spriteTexture;
        }
        public virtual Vector2[] GetTexCoords() {
            return Renderer.DefaultUVCoords;
        }
    }
}