using OpenTK.Mathematics;
using Game.Utils;

namespace Game.Graphics {
    public class OrthoCamera {
        private Matrix4 Projection;
        public double Rotation;
        public Vector3 Position;
        public OrthoCamera(Vector2 size) : this(-size.X / 2, size.X / 2, -size.Y / 2, size.Y / 2) {}
        public OrthoCamera(float left, float right, float bottom, float top) {
            this.Projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1.0f, -1.0f);
            this.Position = Vector3.Zero;
            this.Rotation = 0;
        }
        public void SetProjection(Vector2 size) {
            this.SetProjection(-(size.X / 2), (size.X / 2), -(size.Y / 2), (size.Y / 2));
        }
        public void SetProjection(float left, float right, float bottom, float top) {
            this.Projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1.0f, -1.0f);
            this.Recalculate();    
        }
        public Matrix4 Recalculate() {
            Matrix4 translation = Matrix4.CreateTranslation(this.Position);
            Matrix4 rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians(this.Rotation));
            return Matrix4.Invert(Matrix4.Mult(translation, rotation)) * this.Projection;
        }
    }
}