using System.Collections.Generic;
using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;
using System;

namespace Game.Entity {
    public class EntityManager : IDisposable {
        private List<Entity> entities;
        private List<int> drawableEntities;
        private int playerIndex = -1;
        public EntityManager() {
            this.entities = new List<Entity>();
            this.drawableEntities = new List<int>();
        }
        public void AddEntity(Entity entity) {
            if (entity.GetParrent() == "DrawableEntity") {
                this.drawableEntities.Add(this.entities.Count);
            }
            if (entity.ToString() == "Player") {
                this.playerIndex = this.entities.Count;
            }
            this.entities.Add(entity);
        }
        public Player GetPlayer() {
            if (this.playerIndex >= 0) {
                return (Player)this.entities[this.playerIndex];
            } else {
                GameHandler.Logger.Critical("Player not initalized!");
                return default;
            }
        }
        public void Render(Renderer renderer) {
            foreach (int entityIndex in this.drawableEntities) {
                // Currently player entity visibility range is hardcoded
                if (this.GetPlayer().InRange(this.entities[entityIndex], 5))
                    ((DrawableEntity)this.entities[entityIndex]).Draw(renderer);
            }
        }
        public void Update(double dt) {
            // Todo: Add chunk based updates
            foreach (Entity entity in this.entities) {
                entity.Update(dt);
            }
        }
        public void Dispose() {
            foreach(Entity e in this.entities)
                e.Dispose();
        }
    }
}