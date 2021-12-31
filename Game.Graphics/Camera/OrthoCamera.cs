using OpenTK.Mathematics;
using Game.Utils;
using System.Runtime.InteropServices;
using System;

namespace Game.Graphics {
    public struct CameraQuad {
        public Vector2 Min;
        public Vector2 Max;

        public CameraQuad(float left, float right, float bottom, float top) {
            this.Min = new Vector2(left, top);
            this.Max = new Vector2(right, bottom);
        }
        public void SetQuad(float left, float right, float bottom, float top) {
            this.Min.X = left;
            this.Min.Y = top;
            this.Max.X = right;
            this.Max.Y = bottom;
        }
    }
    public class OrthoCamera {
        private Matrix4 projection;
        private Matrix4 view;
        public Matrix4 ViewProjection { get; set; }
        public IntPtr ViewProjectionPtr { get; protected set; }
        public double Rotation { get; set; }
        public CameraQuad CameraQuad;
        public OrthoCamera(Vector2 size) : this(-size.X / 2, size.X / 2, -size.Y / 2, size.Y /2) {}
        public OrthoCamera(float left, float right, float bottom, float top) {
            this.projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1.0f, -1.0f);
            this.CameraQuad = new CameraQuad(left, right, bottom, top);
            this.view = Matrix4.Identity;
            this.Rotation = 0;
            this.ViewProjection = view * projection;

            // Allocate unmanaged memory for view projection matrix
            this.ViewProjectionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.ViewProjection));
            this.Recalculate(Vector2.Zero);
        }
        public void SetProjection(Vector2 size) { this.SetProjection(-size.X / 2, size.X / 2, -size.Y / 2, size.Y /2); }
        public void SetProjection(float left, float right, float bottom, float top) {
            this.projection = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, 1.0f, -1.0f);
            this.CameraQuad.SetQuad(left, right, bottom, top);
            this.ViewProjection = view * projection;
        }
        public void Recalculate(in Vector2 position) {
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position));
            Matrix4 rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians(this.Rotation));
            Matrix4 mult = Matrix4.Mult(translation, rotation);
            this.view = Matrix4.Invert(mult);
            this.ViewProjection = view * projection;

            // We copy matrix into unmanaged memory, deleting managed version, thisway we avoid garbage collector mem leaks
            Marshal.StructureToPtr<Matrix4>(this.ViewProjection, this.ViewProjectionPtr, true);
        }
        public unsafe IntPtr GetViewProjection() {
            return this.ViewProjectionPtr;
        }
        public void Dispose() {
            Marshal.FreeHGlobal(this.ViewProjectionPtr);
        }
    }
}