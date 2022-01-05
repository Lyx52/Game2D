using OpenTK.Mathematics;
using System;

namespace Game.World {
    public struct Chunk {
        public Vector2i Position { get; private set; }
        public uint[,] Tilemap { get; set; }
        public Chunk(Vector2i position, int width, int height) {
            this.Tilemap = new uint[width, height];
            this.Position = position;
        }
    }
}