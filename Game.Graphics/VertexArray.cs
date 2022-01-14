using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public class VertexArray<TIndexType, TVertexType> : IDisposable 
        where TIndexType : unmanaged 
        where TVertexType : unmanaged 
    {
        public readonly int vaoID;
        public BufferObject<TIndexType> IndexBuffer { get; private set; }
        public BufferObject<TVertexType> VertexBuffer { get; private set; }
        public unsafe VertexArray(VertexLayout layout, int MAX_INDICES, int MAX_VERTICES, in TIndexType[] indices=null, TVertexType[] vertices=null) {
            this.vaoID = GL.GenVertexArray();
            GL.BindVertexArray(this.vaoID);

            this.VertexBuffer = new BufferObject<TVertexType>(sizeof(TVertexType) * MAX_VERTICES, BufferTarget.ArrayBuffer, data:vertices);
            this.IndexBuffer = new BufferObject<TIndexType>(sizeof(TIndexType) * MAX_INDICES, BufferTarget.ElementArrayBuffer, data:indices);
            this.InitVertexAttribs(layout);
        }
        public void Bind() {
            GL.BindVertexArray(this.vaoID);
            this.IndexBuffer.Bind();
            this.VertexBuffer.Bind();
        }
        public void InitVertexAttribs(VertexLayout layout) {
            this.Bind();
            
            foreach (VertexElement element in layout.Elements) {
                GL.EnableVertexAttribArray(element.Index);
                GL.VertexAttribPointer(element.Index, element.Components, element.Type, element.Normalized, layout.Stride, element.Offset);
                GLHelper.CheckGLError("SetAttribPointer");
            }
        }
        public void Dispose() {
            GL.DeleteVertexArray(this.vaoID);
        }
    }
}