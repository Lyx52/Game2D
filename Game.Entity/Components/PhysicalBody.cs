using OpenTK.Mathematics;

namespace Game.Entity {
    public class PhysicalBody : EntityComponent {
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public PhysicalBody(float x, float y) {
            this.Size = new Vector2(32, 32);
            this.Position = new Vector2(x, y);
            this.Rotation = 0.0f;
        }
        public override string ToString() {
            return "PhysicalBody";
        }

    }
}