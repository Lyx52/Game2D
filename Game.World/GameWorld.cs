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
        private static int MAX_CHUNK_DELTA = 2; // Whats the chunks max delta between current player chunk and itself 
        private static int CHUNK_SIZE = 32;
        private static float TILE_SIZE = 4F;
        private static float NOISE_SCALE = 0.0125F;
        private static float TILE_SCALAR = CHUNK_SIZE * TILE_SIZE;
        public EntityManager EntityHandler { get; }
        private SpriteSheet WorldSpriteSheet;
        private Vector2i[] TileMapping = {
            new Vector2i(0, 0),
            new Vector2i(1, 0),
            new Vector2i(2, 0)
        };
        public GameWorld(int seed) {
            this.Chunks = new List<Chunk>();
            this.EntityHandler = new EntityManager();
            Noise.Seed = seed;

            this.WorldSpriteSheet = new SpriteSheet(GameHandler.Renderer.GetTexture("spritesheet"), 3, 1);
            this.EntityHandler.SpawnPlayer(0, 0, Application.Keyboard, Application.Mouse);
            // for (int i = 0; i < 1000; i++) {
            //     TestEntity entity = new TestEntity(i, (float)Rnd.NextDouble());
            //     entity.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            //     this.EntityHandler.AddEntity(entity);
            // }
        }
        public void GenerateGeometry(Vector2 position) {
            Vector2i center = GetChunkPosition(position);
            
            for (int row = -1; row < 2; row++) {
                for (int col = -1; col < 2; col++) {
                    // Generate a new chunk at chunkPos if chunk list dosnt already have it...
                    Vector2i chunkPos = Vector2i.Multiply(center + new Vector2i(col, row), CHUNK_SIZE);
                    if (!this.Chunks.Exists(chunk => chunk.Position == chunkPos))
                        this.Chunks.Add(GenerateChunk(chunkPos));
                }
            }

            // Remove chunk if its out of range
            this.Chunks.RemoveAll(chunk => {
                Vector2i delta = (Vector2i.Divide(chunk.Position, CHUNK_SIZE) - center);
                return Math.Abs(delta.X) >= MAX_CHUNK_DELTA || Math.Abs(delta.Y) >= MAX_CHUNK_DELTA;
            });
        }
        public void Render(Renderer renderer) {
            // Render only visible chunks
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
            this.GenerateGeometry(this.GetPlayer().KinematicBody.Position);
        }
        public Player GetPlayer() {
            return this.EntityHandler.GetPlayer();
        }
        public void Dispose() {
            this.EntityHandler.Dispose();
        }
    }
}