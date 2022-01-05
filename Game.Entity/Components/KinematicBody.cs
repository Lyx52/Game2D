using OpenTK.Mathematics;
namespace Game.Entity {
    public class KinematicBody : PhysicalBody {
        public double Acceleration { get; set; }
        public float Drag { get; set; }
        public Vector2 Velocity { get; set; }
        public KinematicBody(float x, float y) : base(x, y) {
            this.Drag = 0.8f;
            this.Acceleration = 128D;
            this.Velocity = Vector2.Zero;
        }
        public override string ToString() {
            return "KinematicBody";
        }
    }
}