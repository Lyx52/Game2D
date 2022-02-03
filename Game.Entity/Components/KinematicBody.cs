using OpenTK.Mathematics;
namespace Game.Entity {
    public class KinematicBody : PhysicalBody {
        public double Acceleration;
        public float Drag;
        public Vector2 Velocity;
        public KinematicBody(float x, float y) : base(x, y) {
            this.Drag = 0.8f;
            this.Acceleration = 32D;
            this.Velocity = Vector2.Zero;
        }
        public override string ToString() {
            return "KinematicBody";
        }
    }
}