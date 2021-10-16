using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public struct VertexBuffer {
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
            this.Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeInBytes, data);
            this.Unbind();
        }

        public void SetDataFloat(float[] data, int sizeInBytes) {
            this.Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeInBytes, data);
            this.Unbind();
        }
        
        public void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboID);
        }

        public void Unbind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}