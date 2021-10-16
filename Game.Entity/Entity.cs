using System.Collections.Generic;

namespace Game.Entity {
    public abstract class Entity {
        private Dictionary<string, int> componentIndexer;
        private List<object> components;
        public Entity() {
            this.componentIndexer = new Dictionary<string, int>();
            this.components = new List<object>();
        }
        public void AttachComponent<T>(T component, string componentName) {
            if (this.componentIndexer.TryAdd(componentName, this.components.Count)) {
                this.components.Add(component);
            } else {
                GameHandler.Logger.Error($"Component with name <{componentName}> already exists!");
            }
        }
        public int GetComponentIndex(string componentName) {
            if (this.componentIndexer.TryGetValue(componentName, out int index)) {
                return index;
            } else {
                GameHandler.Logger.Error($"Component with name <{componentName}> does not exist!");    
                return -1;
            }
        }
        public T GetComponent<T>(string componentName) {
            return (T)this.components[this.GetComponentIndex(componentName)];
        }
        public void SetComponent<T>(string componentName, T value) {
            this.components[this.GetComponentIndex(componentName)] = value;
        }
    }
}