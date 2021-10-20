namespace Game.Graphics {
    public class AnimatedSprite : SpriteSheet {
        public AnimatedSprite(Texture texture, int cols, int rows) : base(texture, cols, rows) {
            this.Type = SpriteType.ANIMATED_SPRITE;
        }
    }
}