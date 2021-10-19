using System.Collections.Generic;
using Game.Graphics;

namespace Game.Entity {
    public class EntityManager {
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
                return null;
            }
        }
        public void OnRender(Renderer renderer) {
            foreach (int entityIndex in this.drawableEntities) {
                // GameHandler.Logger.Debug($"GC::Generation: {System.GC.GetGeneration(this.entities[entityIndex])}");
                ((DrawableEntity)this.entities[entityIndex]).Draw(renderer);
            }
        }
        public void OnUpdate(double dt) {
            // Todo: Add chunk based updates
            foreach (Entity entity in this.entities) {
                entity.Update(dt);
            }
        }
    }
}