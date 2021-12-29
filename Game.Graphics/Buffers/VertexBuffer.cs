using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public struct VertexBuffer : IDisposable {
        public int Size { get; set; }
        public readonly int vboID { get; }
        public BufferLayout Layout { get; set; }
        public VertexBuffer(int size, float[] data=null) {
            this.Size = size;
            this.vboID = GL.GenBuffer();
            this.Layout = new BufferLayout(null);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, this.Size, data, data == null ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
        }
        public int Stride {
            get {
                return this.Layout.Stride;
            }
        }
        public int VertexSize {
            get {
                return this.Layout.Stride;
            }
        }
        public void SetDataInt(int[] data, int sizeInBytes) {
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeInBytes, data);
        }

        public void SetDataFloat(float[] data, int sizeInBytes) {
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeInBytes, data);
        }
        public void SetDataQuadVertex(IntPtr data, int sizeInBytes) {
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeInBytes, data);
        }
        
        public void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboID);
        }

        public void Unbind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public void Dispose() {
            GL.DeleteBuffer(this.vboID);
        }
    }
}