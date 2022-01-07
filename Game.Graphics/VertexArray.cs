using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public class VertexArray<TIndexType, TVertexType> : IDisposable 
        where TIndexType : unmanaged 
        where TVertexType : unmanaged 
    {
        public readonly int vaoID;
        private BufferObject<TIndexType> IndexBuffer;
        private BufferObject<TVertexType> VertexBuffer;

        public VertexArray() {
            this.vaoID = GL.GenVertexArray();
        }
        public VertexArray(VertexLayout layout, int MAX_INDICES, int MAX_VERTICES) {
            this.IndexBuffer = new BufferObject<TIndexType>(MAX_INDICES, BufferTarget.ElementArrayBuffer);
            this.VertexBuffer = new BufferObject<TVertexType>(MAX_VERTICES, BufferTarget.ArrayBuffer);
        }
        public void Bind() {
            GL.BindVertexArray(this.vaoID);
        }
        public void SetIndexBuffer(BufferObject<TIndexType> buffer) {
            this.IndexBuffer = buffer;
            this.IndexBuffer.Bind();
        }
        public void AddVertexBuffer(BufferObject<TVertexType> buffer, VertexLayout layout) {
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