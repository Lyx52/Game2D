using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Game.Graphics {
    public class SpriteSheet : Sprite {
        public Vector2i TileSize { get; set; }
        public Vector2i SpritesheetSize { get; set; }
        private Dictionary<string, Vector2i> SubSprites;
        private Vector2[][][] tileUV;
        public SpriteSheet(Texture texture, int cols, int rows) : base(texture) {
            this.SpritesheetSize = new Vector2i(cols, rows);
            this.TileSize = Vector2i.Divide(this.TextureSize, this.SpritesheetSize);
            this.Type = SpriteType.SPRITE_SHEET;
            this.tileUV = new Vector2[rows][][];
            this.SubSprites = new Dictionary<string, Vector2i>();
            this.CalculateUVCoords();
        }

        private void CalculateUVCoords() {
            Vector2 sizeNormalized = new Vector2(1.0f / (float)this.SpritesheetSize.X, 1.0f / (float)this.SpritesheetSize.Y);
            float TileX = 0;
            float TileY = 0;
            // Basically create a uv map for each of the tiles
            for (int row = 0; row < this.SpritesheetSize.Y; row++) {
                TileX = 0;
                this.tileUV[row] = new Vector2[this.SpritesheetSize.X][];
                for (int col = 0; col < this.SpritesheetSize.X; col++) {
                    this.tileUV[row][col] = new Vector2[4] {
                        new Vector2(TileX + sizeNormalized.X, TileY + sizeNormalized.Y),
                        new Vector2(TileX + sizeNormalized.X, TileY),
                        new Vector2(TileX, TileY),
                        new Vector2(TileX, TileY + sizeNormalized.Y)
                    };
                    TileX += sizeNormalized.X;
                }
                TileY += sizeNormalized.Y;
            }
            
            // TODO: Check if we still need to reverse the array
            // Array.Reverse(this.tileUV);
        }
        public void AddNamedSubSprite(string spriteName, int x, int y) {
            if (this.SubSprites.ContainsKey(spriteName)) {
                GameHandler.Logger.Error($"Trying to add already defined named SubSprite({spriteName})!");
            } else {
                this.SubSprites.Add(spriteName, new Vector2i(x, y));
            }
        }
        public Vector2[] GetNamedSubSprite(string spriteName) {
            if (this.SubSprites.ContainsKey(spriteName)) {
                return this.GetSubSprite(this.SubSprites[spriteName]);
            } else {
                GameHandler.Logger.Error($"Trying to get undefined named SubSprite({spriteName})!");
                return this.GetTexCoords();
            }    
        }
        public Vector2[] GetSubSprite(Vector2i pos) {
            return this.GetSubSprite(pos.X, pos.Y);
        }
        public Vector2[] GetSubSprite(int x, int y) {
            try {
                return this.tileUV[y][x];
            } catch (IndexOutOfRangeException) {
                GameHandler.Logger.Error($"Trying to access SubSprite out of Spritesheet range({x}, {y})!");
                return this.GetTexCoords();
            }
        }
    }
}