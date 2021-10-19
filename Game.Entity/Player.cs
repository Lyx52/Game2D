using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;

namespace Game.Entity {
    public class Player : DrawableEntity {
        public Player(float x, float y) : base() {
            this.AttachComponent<KinematicBody>(new KinematicBody(x, y), "kinematic_body");
            this.AttachComponent<EntityController>(new EntityController(this), "controller");
            this.AttachComponent<OrthoCamera>(new OrthoCamera(-1, 1, -1, 1), "camera");
        }
        public override void Draw(Renderer renderer) {
            renderer.DrawQuad(this.PhysicalBody.Position, this.PhysicalBody.Size, this.Texture, this.TexCoords, this.MaskColor, rotation:this.PhysicalBody.Rotation);
        }
        public EntityController Controller {
            get { return this.GetComponent<EntityController>("controller"); }
        }
        public KinematicBody PhysicalBody {
            get { return this.GetComponent<KinematicBody>("kinematic_body"); }
        }
        public OrthoCamera Camera {
            get { return this.GetComponent<OrthoCamera>("camera"); }
        }
        public override void Update(double dt) {
            // Rotate sprite towards mouse
            this.PhysicalBody.Rotation = MathUtils.LookAt(this.PhysicalBody.Position, this.Controller.GlobalMousePosition);

            // Acceleration vector is equal to UP/DOWN key multiplied by acceleration
            Vector2 acceleration = new Vector2(0, this.Controller.GetDirectional().Y * (float)(dt *this.PhysicalBody.Acceleration));
            
            // We rotate acceleration vector to sprite angle and add it to velocity
            this.PhysicalBody.Velocity += MathUtils.Rotate(acceleration, this.PhysicalBody.Rotation * MathUtils.Deg2Rad);
            
            // We add velocity to position
            this.PhysicalBody.Position += this.PhysicalBody.Velocity;

            // We add drag to velocity
            this.PhysicalBody.Velocity *= this.PhysicalBody.Drag;

            // Recalculate camera matrix and update it in renderer
            this.Camera.Recalculate(this.PhysicalBody.Position);
            GameHandler.Renderer.PtrViewProjection = this.Camera.PtrViewProjection;
        }
        public override string ToString() {
            return "Player";
        }
        public override string GetParrent() {
            return base.ToString();
        }
    }
}