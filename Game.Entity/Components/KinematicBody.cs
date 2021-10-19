using OpenTK.Mathematics;
namespace Game.Entity {
    public class KinematicBody {
        public double Acceleration { get; set; }
        public float Drag { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public KinematicBody(float x, float y) {
            this.Position = new Vector2(x, y);
            this.Rotation = 0.0f;
            this.Drag = 0.8f;
            this.Acceleration = 1.0D;
            this.Size = Vector2.One;
            this.Velocity = Vector2.Zero;
        }
    }
}