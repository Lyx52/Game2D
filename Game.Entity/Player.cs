using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;

namespace Game.Entity {
    public class Player : DrawableEntity {
        public Player(float x, float y) : base() {
            this.AttachComponent<KinematicBody>(new KinematicBody(x, y), "KinematicBody");
            this.AttachComponent<EntityController>(new EntityController(this), "Controller");
            this.AttachComponent<OrthoCamera>(new OrthoCamera(-1, 1, -1, 1), "Camera");
        }
        public override void Draw(Renderer renderer) {
            renderer.DrawQuad(this.KinematicBody.Position, this.KinematicBody.Size, this.Texture, this.TexCoords, this.MaskColor, rotation:this.KinematicBody.Rotation);
        }
        public EntityController Controller {
            get { return this.GetComponent<EntityController>("Controller"); }
        }
        public KinematicBody KinematicBody {
            get { return this.GetComponent<KinematicBody>("KinematicBody"); }
        }
        public OrthoCamera Camera {
            get { return this.GetComponent<OrthoCamera>("Camera"); }
        }
        public override void Update(double dt) {
            // Rotate sprite towards mouse
            this.KinematicBody.Rotation = MathUtils.LookAt(this.KinematicBody.Position, this.Controller.GlobalMousePosition);

            // Acceleration vector is equal to UP/DOWN key multiplied by acceleration
            Vector2 acceleration = new Vector2(0, this.Controller.GetDirectional().Y * (float)(dt *this.KinematicBody.Acceleration));
            
            // We rotate acceleration vector to sprite angle and add it to velocity
            this.KinematicBody.Velocity += MathUtils.Rotate(acceleration, this.KinematicBody.Rotation * MathUtils.Deg2Rad);
            
            // We add velocity to position
            this.KinematicBody.Position += this.KinematicBody.Velocity;

            // We add drag to velocity
            this.KinematicBody.Velocity *= this.KinematicBody.Drag;

            // Recalculate camera matrix and update it in renderer
            this.Camera.Recalculate(this.KinematicBody.Position);
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