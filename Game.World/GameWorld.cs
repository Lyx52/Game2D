using Game.Entity;
using Game.Core;
using Game.Graphics;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using SimplexNoise;

namespace Game.World {
    public class GameWorld : IDisposable {
        public List<Chunk> Chunks;
        private static int CHUNK_SIZE = 32;
        private static float TILE_SIZE = 8F;
        private static float NOISE_SCALE = 0.05F;
        private static float TILE_SCALAR = CHUNK_SIZE * TILE_SIZE;
        public EntityManager EntityHandler { get; }
        private Random Rnd;
        private SpriteSheet WorldSpriteSheet;
        private Vector2i[] TileMapping = {
            new Vector2i(0, 0),
            new Vector2i(1, 0),
            new Vector2i(2, 0)
        };
        public Vector2i LastPlayerChunk = Vector2i.Zero;
        public GameWorld(int seed) {
            this.Chunks = new List<Chunk>();
            this.EntityHandler = new EntityManager();
            this.Rnd = new Random(seed);
            Noise.Seed = seed;

            this.WorldSpriteSheet = new SpriteSheet(GameHandler.Renderer.GetTexture("spritesheet"), 3, 1);
            this.EntityHandler.SpawnPlayer(0, 0, Application.Keyboard, Application.Mouse);
            // NOTE: Each entity is about 470-500 bytes
            // for (int i = 0; i < 100; i++) {
            //     TestEntity entity = new TestEntity(i, (float)Rnd.NextDouble());
            //     entity.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            //     this.EntityHandler.AddEntity(entity);
            // }
        }
        public void GenerateGeometry(Vector2 position) {
            Vector2i center = GetChunkPosition(position);
            Vector2i size = new Vector2i((int)CHUNK_SIZE, (int)CHUNK_SIZE);
            this.Chunks.Add(GenerateChunk((center + new Vector2i(-1, 0)) * size));
            this.Chunks.Add(GenerateChunk((center + new Vector2i(1, 0)) * size));
            this.Chunks.Add(GenerateChunk((center + new Vector2i(0, 1)) * size));
            this.Chunks.Add(GenerateChunk((center + new Vector2i(0, -1)) * size));

            this.Chunks.Add(GenerateChunk((center + new Vector2i(-1, -1)) * size));
            this.Chunks.Add(GenerateChunk((center + new Vector2i(1, 1)) * size));
            
            this.Chunks.Add(GenerateChunk(center * size));
            
            this.Chunks.Add(GenerateChunk((center + new Vector2i(-1, 1)) * size));
            this.Chunks.Add(GenerateChunk((center + new Vector2i(1, -1)) * size));
        }
        public void Render(Renderer renderer) {
            foreach(Chunk chunk in this.Chunks) {
                this.DrawChunk(renderer, chunk);
            }
            this.EntityHandler.Render(renderer);
        }
        public Chunk GenerateChunk(Vector2i position, int offsetX=0) {
            Chunk chunk = new Chunk(position, CHUNK_SIZE, CHUNK_SIZE);
            for (int row = 0; row < CHUNK_SIZE; row++) {
                for (int col = 0; col < CHUNK_SIZE; col++) {
                    chunk.Tilemap[row, col] = this.GetTileSprite(row + position.Y, col + position.X);
                }
            }
            return chunk;
        }
        public uint GetTileSprite(int row, int col) {
            float noise = Noise.CalcPixel2D(row, col, NOISE_SCALE);
            return (uint)(noise <= 75F ? 0 : noise <= 100F ? 2 : 1);
        }
        private void DrawChunk(Renderer renderer, Chunk chunk) {
            for (int row = 0; row < chunk.Tilemap.GetLength(0); row++) {
                for (int col = 0; col < chunk.Tilemap.GetLength(1); col++) {
                    this.DrawTile(renderer, new Vector2(col, row) + chunk.Position, this.TileMapping[chunk.Tilemap[row, col]]);
                }
            }
        }
        private void DrawTile(Renderer renderer, Vector2 position, Vector2i subsprite) {
            renderer.DispatchQuad(new DrawQuad2D(TILE_SIZE * position, new Vector2(TILE_SIZE), this.WorldSpriteSheet, subsprite));    
        }
        public static Vector2i GetChunkPosition(Vector2 position) {
            return new Vector2i((int)Math.Floor(position.X / TILE_SCALAR), (int)Math.Floor(position.Y / TILE_SCALAR));
        }
        public void Update(double dt) {
            this.EntityHandler.Update(dt);
            this.Chunks.Clear();
            this.GenerateGeometry(this.GetPlayer().KinematicBody.Position);
            GameHandler.Logger.Debug($"PlayerCurrentChunk{GetChunkPosition(this.GetPlayer().KinematicBody.Position)}");
        }
        public Player GetPlayer() {
            return this.EntityHandler.GetPlayer();
        }
        public void Dispose() {
            this.EntityHandler.Dispose();
        }
    }
}