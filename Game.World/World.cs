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
        private static int MIN_CHUNKFILE_SIZE = 0;
        private static int MIN_WORLDFILE_SIZE = 0;
        private static int MAX_CHUNK_DELTA = 2; // Whats the chunks max delta position between current player chunk and itself 
        private static int CHUNK_SIZE = 32;
        private static float TILE_SIZE = 16F;
        private static float NOISE_SCALE = 0.0125F;
        private static float TILE_SCALAR = CHUNK_SIZE * TILE_SIZE;
        private string WorldName;
        public EntityManager EntityHandler { get; }
        private SpriteSheet WorldSpriteSheet;
        private FileStream WorldStream;
        private FileStream ChunkStream;
        private DirectoryInfo SavesDirectory;
        private DirectoryInfo WorldDirectory;
        private CompoundTag WorldInfo;
        private CompoundTag ChunkInfo;
        private bool IsCreated = false;
        private Vector2i[] TileMapping = {
            new Vector2i(0, 0),
            new Vector2i(1, 0),
            new Vector2i(2, 0)
        };
        public World(int seed, string worldName) {
            this.Chunks = new List<Chunk>();
            this.EntityHandler = new EntityManager();
            Noise.Seed = seed;
            this.WorldName = worldName;

            this.WorldSpriteSheet = new SpriteSheet(GameHandler.Renderer.GetTexture("spritesheet"), 3, 1);
            this.EntityHandler.SpawnPlayer(0, 0, Application.Keyboard, Application.Mouse);
            

            // Load or create the world
            this.SavesDirectory = Directory.CreateDirectory(Path.Combine(IOUtils.GetCWD(), "saves"));
            this.WorldDirectory = Array.Find(this.SavesDirectory.GetDirectories(), saveDir => {
                return saveDir.Name == this.WorldName;
            });

            if (this.WorldDirectory == default(DirectoryInfo)) {
                // Directory was not found, so the world doesn't exist!
                this.Create();
            } else {
                // Try to load the world
                this.Load();
            }
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
                if (mustBeRemoved) this.SaveChunk(chunk);
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
        public void SaveChunk(Chunk chunk) {
            CompoundTag chunkTag = new CompoundTag($"Chunk_{chunk.Position}");
            chunkTag.AddTag(new Vector2iTag("Position", chunk.Position));
            chunkTag.AddTag(new ByteArrayTag("Data", ArrayUtils.Flatten(chunk.Tilemap)));

            // Add absolute position in binary file
            this.ChunkInfo.AddTag(new LongTag(chunkTag.Name, this.ChunkStream.Position));
            chunkTag.WriteTag(this.ChunkStream);
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
        public void Create() {
            GameHandler.Logger.Debug($"Creating world {this.WorldName}!");
            this.WorldDirectory = this.SavesDirectory.CreateSubdirectory(this.WorldName);
            
            this.WorldStream = IOUtils.OpenFileStream(Path.Combine(this.WorldDirectory.FullName, "WorldData.bin"));
            this.ChunkStream = IOUtils.OpenFileStream(Path.Combine(this.WorldDirectory.FullName, "ChunkData.bin"));

            // Tags for storing essential world data
            this.ChunkInfo = new CompoundTag("ChunkInfo");
            this.WorldInfo = new CompoundTag("WorldInfo");

            // for (int i = 0; i < 1000; i++) {
            //     TestEntity entity = new TestEntity(i, (float)Rnd.NextDouble());
            //     entity.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            //     this.EntityHandler.AddEntity(entity);
            // }
        }
        public void Save() {
            GameHandler.Logger.Debug($"Saving world {this.WorldName}!");
            
            // Write world info stream
            this.WorldInfo.AddTag(this.GetPlayer().GetPlayerTag());
            this.WorldInfo.AddTag(this.ChunkInfo);
            this.WorldInfo.AddTag(new StringTag("WorldSeed", Noise.Seed.ToString()));
            this.WorldInfo.WriteTag(this.WorldStream);
        }
        public void Load() {
            GameHandler.Logger.Debug($"Loading world {this.WorldName}!");
            foreach (FileInfo file in  this.WorldDirectory.GetFiles()) {
                if (file.Name.Equals("ChunkData.bin")) {
                    this.ChunkStream = file.Open(FileMode.Open, FileAccess.ReadWrite);
                } else if (file.Name.Equals("WorldData.bin")) {
                    this.WorldStream = file.Open(FileMode.Open, FileAccess.ReadWrite);
                }
            }
            this.IsCreated = (this.ChunkStream != default(FileStream) && this.ChunkStream.Length > MIN_CHUNKFILE_SIZE) && (this.WorldStream != default(FileStream) && this.WorldStream.Length > MIN_WORLDFILE_SIZE);
            if (this.IsCreated) {
                // Load
                this.WorldInfo = (CompoundTag)Tag.ReadTag(this.WorldStream);
                
                // Load player data
                GameHandler.Logger.Assert(this.WorldInfo.Contains("Player"), "WorldInfo tag doesn't contain Player key!");
                this.GetPlayer().LoadPlayerData(this.WorldInfo.GetCompoundTag("Player"));
                
                GameHandler.Logger.Assert(this.WorldInfo.Contains("ChunkInfo"), "WorldInfo tag doesn't contain ChunkInfo key!");
                this.ChunkInfo = this.WorldInfo.GetCompoundTag("ChunkInfo");
            } else {
                // Cannot load corrupted or incomplete world save, so create a new one
                GameHandler.Logger.Warn($"Cannot load world {this.WorldName}, either corrupted or incomplete!");
                this.Create();
            }
        }
        public void Dispose() {
            this.Save();
            this.WorldStream.Close();
            this.ChunkStream.Close();
            this.EntityHandler.Dispose();
        }
    }
}