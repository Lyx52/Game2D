using OpenTK.Mathematics;
using Game.Utils;
using System;

namespace Game.Entity {
    public class OrthoCamera {
        private Matrix4 projection;
        private Matrix4 view;
        public Matrix4 ViewProjection { get; set; }
        public double Rotation { get; set; }
        public IntPtr PtrViewProjection { get; set; }
        public OrthoCamera(float left, float right, float bottom, float top) {
            this.projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1, -1);
            this.view = Matrix4.Identity;
            this.Rotation = 0;
            this.ViewProjection = view * projection;
            this.PtrViewProjection = IOUtils.GetObjectPtr(this.ViewProjection);
        }
        public void SetProjection(float left, float right, float bottom, float top) {
            this.projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1, -1);
            this.ViewProjection = view * projection;
        }
        public void Recalculate(Vector2 position) {

            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position));
            Matrix4 rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians(this.Rotation));
            Matrix4 mult = Matrix4.Mult(translation, rotation);
            this.view = Matrix4.Invert(mult);
            this.ViewProjection = view * projection;
            this.PtrViewProjection = IOUtils.GetObjectPtr(this.ViewProjection);
        }
    }
}