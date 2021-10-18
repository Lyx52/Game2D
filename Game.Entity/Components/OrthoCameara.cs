using OpenTK.Mathematics;
using Game.Utils;
using Game.Graphics;

namespace Game.Entity {
    public struct OrthoCamera {
        private Matrix4 projection;
        private Matrix4 view;
        private Matrix4 viewProjection;
        public double rotation { get; set; }
        public Vector2 Position { get; set; }
        public OrthoCamera(float left, float right, float bottom, float top, Vector2 position) {
            this.projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1, -1);
            this.view = Matrix4.Identity;
            this.rotation = 0;
            this.viewProjection = view * projection;
            this.Position = position;
        }
        public void SetProjection(float left, float right, float bottom, float top) {
            this.projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1, -1);
            this.viewProjection = view * projection;
        }
        public void Recalculate() {
            /**
                Todo check why this.Position is not updating...
            **/

            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(this.Position));
            Matrix4 rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians(this.rotation));
            Matrix4 mult = Matrix4.Mult(translation, rotation);
            this.view = Matrix4.Invert(mult);
            this.viewProjection = view * projection;
        }
        public void SetPosition(Vector2 position) {
            this.Position = position;
        }
        public void SetViewMatrix(Renderer renderer) {
            this.Recalculate();
            renderer.SetViewProjection(this.viewProjection);
        }
    }
}