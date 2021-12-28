using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;

namespace Game.Entity {
    public class Player : DrawableEntity {
        public Player(float x, float y) : base() {
            this.AttachComponent(new KinematicBody(x, y), "KinematicBody");
            this.AttachComponent(new EntityController(this), "Controller");
            this.Layer = RenderLayer.LAYER_2;
        }
        public override void Draw(Renderer renderer) {
            renderer.DispatchQuad(this.GetRenderQuad(this.KinematicBody));         
        }
        public EntityController Controller {
            get { return (EntityController)this.GetComponent("Controller"); }
        }
        public KinematicBody KinematicBody {
            get { return (KinematicBody)this.GetComponent("KinematicBody"); }
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
        }
        public override string ToString() {
            return "Player";
        }
        public override string GetParrent() {
            return base.ToString();
        }
    }
}