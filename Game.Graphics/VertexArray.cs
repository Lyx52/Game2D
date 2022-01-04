using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public class VertexArray<T1, T2> : IDisposable 
        where T1 : unmanaged 
        where T2 : unmanaged 
    {
        public readonly int vaoID;
        private BufferObject<T1> IndexBuffer;
        private BufferObject<T2> VertexBuffer;

        public VertexArray() {
            this.vaoID = GL.GenVertexArray();
        }
        public void Bind() {
            GL.BindVertexArray(this.vaoID);
        }
        public void SetIndexBuffer(BufferObject<T1> buffer) {
            this.IndexBuffer = buffer;
            this.IndexBuffer.Bind();
        }
        public void AddVertexBuffer(BufferObject<T2> buffer, VertexLayout layout) {
            this.Bind();
            buffer.Bind();

            foreach (VertexElement element in layout.Elements) {
                GL.EnableVertexAttribArray(element.Index);
                GL.VertexAttribPointer(element.Index, element.Components, element.Type, element.Normalized, layout.Stride, element.Offset);
                GLHelper.CheckGLError("SetAttribPointer");
            }

            this.VertexBuffer = buffer;
        }
        public void Dispose() {
            GL.DeleteVertexArray(this.vaoID);
        }
    }
}