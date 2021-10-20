using Game.Graphics;
using OpenTK.Mathematics;

namespace Game.Entity {
    public class TestEntity : DrawableEntity {
        public TestEntity(float x, float y) {
            this.AttachComponent<KinematicBody>(new KinematicBody(x, y), "KinematicBody");
        }
        public override void Update(double dt) {

        }
        public override void Draw(Renderer renderer)
        {
            renderer.DrawQuad(this.KinematicBody.Position, this.KinematicBody.Size, this.Texture, this.TexCoords, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), rotation:this.KinematicBody.Rotation);
            this.KinematicBody.Rotation += 1.0f;
        }
        public KinematicBody KinematicBody {
            get { return this.GetComponent<KinematicBody>("KinematicBody"); }
        }
        public override string GetParrent() {
            return base.ToString();
        }
    }
}