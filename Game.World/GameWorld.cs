using Game.Entity;
using Game.Core;
using Game.Graphics;
using System;
using OpenTK.Mathematics;
using SimplexNoise;

namespace Game.World {
    public class GameWorld : IDisposable {
        public EntityManager EntityHandler { get; }
        private Random Rnd;
        private SpriteSheet WorldSpriteSheet;
        private Vector2 TileSize = new Vector2(8, 8);
        private float[,] NoiseMap = new float[128, 128];
        private Vector2i[] TileMapping = {
            new Vector2i(0, 0),
            new Vector2i(1, 0),
            new Vector2i(2, 0)
        };
        public int[,] TileMap;
        public GameWorld(int seed) {
            this.EntityHandler = new EntityManager();
            this.Rnd = new Random(seed);
            Noise.Seed = seed;
            NoiseMap = Noise.Calc2D(NoiseMap.GetLength(0), NoiseMap.GetLength(1), 0.015F);
            TileMap = new int[NoiseMap.GetLength(0), NoiseMap.GetLength(1)];

            for (int row = 0; row < NoiseMap.GetLength(0); row++) {
                for (int col = 0; col < NoiseMap.GetLength(1); col++) {
                    TileMap[row, col] = this.GetTileSprite(row, col);
                }
            }
            this.WorldSpriteSheet = new SpriteSheet(GameHandler.Renderer.GetTexture("spritesheet"), 3, 1);
            this.EntityHandler.SpawnPlayer(0, 0, Application.Keyboard, Application.Mouse);
            // NOTE: Each entity is about 470-500 bytes
            // for (int i = 0; i < 100; i++) {
            //     TestEntity entity = new TestEntity(i, (float)Rnd.NextDouble());
            //     entity.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            //     this.EntityHandler.AddEntity(entity);
            // }
        }
        public void Render(Renderer renderer) {
            for (int row = 0; row < NoiseMap.GetLength(0); row++) {
                for (int col = 0; col < NoiseMap.GetLength(1); col++) {
                    this.DrawTile(renderer, new Vector2(col, row), this.TileMapping[this.TileMap[row, col]]);
                }
            }
            this.EntityHandler.Render(renderer);
        }
        public int GetTileSprite(int row, int col) {
            // Test code!
            return Math.Abs(this.NoiseMap[row, col]) <= 75F ? 0 : Math.Abs(this.NoiseMap[row, col]) <= 100F ? 2 : 1;
        }
        private void DrawTile(Renderer renderer, Vector2 position, Vector2i subsprite) {
            renderer.DispatchQuad(new DrawQuad2D(this.TileSize * position, TileSize, this.WorldSpriteSheet, subsprite));    
        }
        public void Update(double dt) {
            this.EntityHandler.Update(dt);
        }
        public Player GetPlayer() {
            return this.EntityHandler.GetPlayer();
        }
        public void Dispose() {
            this.EntityHandler.Dispose();
        }
    }
}