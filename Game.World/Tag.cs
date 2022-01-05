using System.Collections.Generic;

namespace Game.World {
    public class BaseTag {
        public string Name { get; set; }
    }
    public class StringTag : BaseTag {
        public string Value;
    }
    public class CompoundTag : BaseTag {
        public Dictionary<string, BaseTag> Tags { get; set; }
    }
}