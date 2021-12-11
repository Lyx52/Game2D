using Game.Graphics;
using OpenTK.Mathematics;

namespace Game.Entity {
    public class TestEntity : DrawableEntity {
        public TestEntity(float x, float y) {
            this.AttachComponent(new KinematicBody(x, y), "KinematicBody");
            this.Layer = RenderLayer.LAYER_1;
        }
        public override void Update(double dt) {

        }
        public override void Draw(Renderer renderer)
        {
            renderer.DispatchQuad(this.GetRenderQuad(this.KinematicBody)); 
            this.KinematicBody.Rotation += 1.0f;
        }
        public KinematicBody KinematicBody {
            get { return (KinematicBody)this.GetComponent("KinematicBody"); }
        }
        public override string GetParrent() {
            return base.ToString();
        }
    }
}