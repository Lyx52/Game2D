using Game.Graphics;
using OpenTK.Mathematics;

namespace Game.Entity {
    public class TestEntity : DrawableEntity {
        public TestEntity(float x, float y) {
            this.AttachComponent<KinematicBody>(new KinematicBody(x, y), "kinematicBody");
        }
        public override void Update(double dt) {

        }
        public override void Draw(Renderer renderer)
        {
            renderer.DrawQuad(this.PhysicalBody.Position, this.PhysicalBody.Size, this.Texture, this.TexCoords, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), rotation:this.PhysicalBody.Rotation);
            this.PhysicalBody.Rotation += 1.0f;
        }
        public KinematicBody PhysicalBody {
            get { return this.GetComponent<KinematicBody>("kinematicBody"); }
        }
        public override string GetParrent() {
            return base.ToString();
        }
    }
}