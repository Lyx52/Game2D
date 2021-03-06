using OpenTK.Mathematics;

namespace Game.Graphics {
    public enum SpriteType {
        SPRITE,
        SPRITE_SHEET,
        ANIMATED_SPRITE
    }
    public class Sprite {
        public Texture SpriteTexture { get; set; }
        public SpriteType Type { get; set; }
        public int TextureWidth { get { return this.SpriteTexture.Width; } }
        public int TextureHeight { get { return this.SpriteTexture.Height; } }
        public Vector2i TextureSize { get; private set; }
        public Sprite(in Texture texture) {
            this.SpriteTexture = texture;
            this.Type = SpriteType.SPRITE;
            this.TextureSize = new Vector2i(this.TextureWidth, this.TextureHeight);
        }
        public virtual Vector2[] GetTexCoords() {
            return Renderer.DefaultUVCoords;
        }
    }
}