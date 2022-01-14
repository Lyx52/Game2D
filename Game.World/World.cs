using Game.Entity;
using Game.Core;
using Game.Graphics;
using Game.Utils;

using System;
using System.Collections.Generic;
using System.IO;

using OpenTK.Mathematics;
using SimplexNoise;


namespace Game.World {
    public struct Chunk {
        public Vector2i Position { get; private set; }
        public byte[,] Tilemap { get; set; }
        public Chunk(Vector2i position, int width, int height) {
            this.Tilemap = new byte[width, height];
            this.Position = position;
        }
    }
    public class World : IDisposable {
        public List<Chunk> Chunks;
        private static int MAX_CHUNK_DELTA = 2; // Whats the chunks max delta position between current player chunk and itself 
        private static int CHUNK_SIZE = 32;
        private static float TILE_SIZE = 2F;
        private static float NOISE_SCALE = 0.0125F;
        private static float TILE_SCALAR = CHUNK_SIZE * TILE_SIZE;
        public EntityManager EntityHandler { get; }
        private SpriteSheet WorldSpriteSheet;
        private FileStream WorldStream;
        private FileStream ChunkStream;
        private bool IsCreated = false;
        private Vector2i[] TileMapping = {
            new Vector2i(0, 0),
            new Vector2i(1, 0),
            new Vector2i(2, 0)
        };
        public World(int seed, string fileName) {
            this.Chunks = new List<Chunk>();
            this.EntityHandler = new EntityManager();
            Noise.Seed = seed;

            this.WorldSpriteSheet = new SpriteSheet(GameHandler.Renderer.GetTexture("spritesheet"), 3, 1);
            this.EntityHandler.SpawnPlayer(0, 0, Application.Keyboard, Application.Mouse);
            this.IsCreated = File.Exists(fileName) && File.Exists("chunks.bin");
            if (this.IsCreated) {
                this.WorldStream = IOUtils.OpenReadStream(fileName);
                this.ChunkStream = IOUtils.OpenReadStream("chunks.bin");
                this.LoadWorld();
                this.WorldStream.Close();
            }
            this.WorldStream = IOUtils.OpenWriteStream(fileName);
            this.ChunkStream = IOUtils.OpenWriteStream("chunks.bin");

            // for (int i = 0; i < 1000; i++) {
            //     TestEntity entity = new TestEntity(i, (float)Rnd.NextDouble());
            //     entity.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            //     this.EntityHandler.AddEntity(entity);
            // }
        }
        public void GenerateGeometry(Vector2 position) {
            Vector2i center = GetChunkPosition(position);
            //GameHandler.Profiler.StartSection("ChunkGeneration");
            for (int row = -(MAX_CHUNK_DELTA - 1); row < MAX_CHUNK_DELTA; row++) {
                for (int col = -(MAX_CHUNK_DELTA - 1); col < MAX_CHUNK_DELTA; col++) {
                    // Generate a new chunk at chunkPos if chunk list dosnt already have it...
                    Vector2i chunkPos = Vector2i.Multiply(center + new Vector2i(col, row), CHUNK_SIZE);
                    if (this.Chunks.Exists(chunk => chunk.Position == chunkPos))
                        continue;
                    this.Chunks.Add(GenerateChunk(chunkPos));
                }
            }
            //GameHandler.Profiler.EndSection("ChunkGeneration");
            //GameHandler.Profiler.StartSection("ChunkRemoval");
            // Remove chunk if its out of range
            this.Chunks.RemoveAll(chunk => {
                Vector2i delta = (Vector2i.Divide(chunk.Position, CHUNK_SIZE) - center);
                bool mustBeRemoved = Math.Abs(delta.X) >= MAX_CHUNK_DELTA || Math.Abs(delta.Y) >= MAX_CHUNK_DELTA;
                
                // We save the chunks we remove from range
                // if (mustBeRemoved)
                //     this.SaveChunk(this.ChunkStream, chunk);

                return mustBeRemoved;
            });
            //GameHandler.Profiler.EndSection("ChunkRemoval");
        }
        public void Render(in Renderer renderer) {
            // Render only visible chunks
            //GameHandler.Profiler.StartSection("ChunkRendering");
            foreach (Chunk chunk in this.Chunks) {
                this.DrawChunk(renderer, chunk);
            }
            //GameHandler.Profiler.EndSection("ChunkRendering");
            this.EntityHandler.Render(renderer);
        }
        public void SaveChunk(FileStream stream, Chunk chunk) {
            CompoundTag chunkTag = new CompoundTag($"Chunk_{chunk.Position}");
            chunkTag.AddTag(new CompoundTag("ChunkPosition", new List<Tag>() {
                new IntTag("X", chunk.Position.X),
                new IntTag("Y", chunk.Position.Y)
            }));
            chunkTag.AddTag(new ByteArrayTag("ChunkData", ArrayUtils.Flatten(chunk.Tilemap)));
            chunkTag.WriteTag(stream);
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
        public byte GetTileSprite(int row, int col) {
            float noise = Noise.CalcPixel2D(row, col, NOISE_SCALE);
            return (byte)(noise <= 75F ? 0 : noise <= 125F ? 2 : 1);
        }
        private void DrawChunk(in Renderer renderer, Chunk chunk) {
            // GameHandler.Profiler.StartSection("SingleChunkRendering");
            for (int row = 0; row < chunk.Tilemap.GetLength(0); row++) {
                for (int col = 0; col < chunk.Tilemap.GetLength(1); col++) {
                    this.DrawTile(renderer, new Vector2(col, row) + chunk.Position, this.TileMapping[chunk.Tilemap[row, col]]);
                }
            }
            // GameHandler.Profiler.EndSection("SingleChunkRendering");
        }
        private void DrawTile(in Renderer renderer, Vector2 position, Vector2i subsprite) {
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
        public void LoadWorld() {
            List<Tag> save = Tag.ReadTags(this.WorldStream);
            foreach (Tag tag in save) {
                switch(tag.Name) {
                    case "player": {
                        this.GetPlayer().LoadPlayerData((CompoundTag)tag);
                    } break;
                    default: {
                        GameHandler.Logger.Warn($"Unhandled tag in save file {tag.Name}!");
                    } break;
                }
            }
        }
        public void Dispose() {
            List<Tag> WorldTags = new List<Tag>(){
                this.GetPlayer().GetPlayerTag(),
                new StringTag("WorldSeed", Noise.Seed.ToString()),
                new EndTag()
            };
            GameHandler.Logger.Debug($"\n{Tag.TagsAsString(WorldTags)}");
            Tag.WriteTags(this.WorldStream, WorldTags);
            this.WorldStream.Close();
            this.ChunkStream.Close();
            this.EntityHandler.Dispose();
        }
    }
}