using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
     public struct IndexBuffer {
        public int Count { get; set; }
        public readonly int eboID { get; }
        public BufferLayout Layout { get; set; }

        public IndexBuffer(int count, uint[] data=null) {
            this.Count = count;
            this.eboID = GL.GenBuffer();
            this.Layout = new BufferLayout(null);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.Count, data, data == null ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
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

        public void SetData(uint[] data, int sizeInBytes) {
            this.Bind();
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)0, sizeInBytes, data);
            this.Unbind();
        }

        public void Bind() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboID);
        }

        public void Unbind() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }  
    }
}