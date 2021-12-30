using System.Collections.Generic;
using System;
namespace Game.Entity {
        public abstract class Entity : IDisposable {
        private SortedList<string, EntityComponent> components;
        public Guid ID { get; set; }
        public long LastUpdated = 0;
        public Entity() {
            this.ID = Guid.NewGuid();
            this.components = new SortedList<string, EntityComponent>();
        }
        public void AttachComponent(EntityComponent component) {
            this.AttachComponent(component, component.GetType().Name);
        }
        public void AttachComponent(EntityComponent component, string componentName) {
            if (this.components.ContainsKey(componentName)) {
                GameHandler.Logger.Error($"Component with name <{componentName}> already exists!");
            } else {
                this.components.Add(componentName, component);
            }
        }
        public bool ContainsComponent(string componentName) {
            return this.components.ContainsKey(componentName);
        }
        public EntityComponent GetComponent(Type type) {
            return this.GetComponent(type.ToString());
        }
        public EntityComponent GetComponent(string componentName) {
            if (this.components.ContainsKey(componentName)) {
                return this.components[componentName];
            } else {
                GameHandler.Logger.Error($"Component with name <{componentName}> dosn't exist!");
                return default(EntityComponent);
            }
        }
        public void SetComponent(string componentName, EntityComponent value) {
            if (this.components.ContainsKey(componentName)) {
                this.components[componentName] = value;
            } else {
                GameHandler.Logger.Error($"Component with name <{componentName}> dosn't exist!");
            }
        }
        public virtual void Update(double dt) {
            // Update time when entity was last updated
            this.LastUpdated = DateTime.Now.Ticks;
        }
        
        public override string ToString() {
            return "Entity";
        }
        public virtual string GetParrent() {
            return "";
        }
        public bool Equals(Entity entity) {
            return entity.ID == this.ID;
        }
        public abstract bool InRange(Entity target, float range);
        public void Dispose() {}
    }
}