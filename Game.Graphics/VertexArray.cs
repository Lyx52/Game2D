using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;

namespace Game.Graphics {
    public class VertexArray<T1, T2> : IDisposable 
        where T1 : unmanaged 
        where T2 : unmanaged 
    {
        public readonly int vaoID;
        private BufferObject<T1> IndexBuffer;
        private BufferObject<T1> VertexBuffer;

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
        public void AddVertexBuffer(BufferObject<T2> buffer, BufferLayout layout) {
            if (!layout.IsValid()) {
                GameHandler.Logger.Error("When adding a VertexBuffer it must have a valid layout!");
            }

            this.Bind();
            buffer.Bind();
            foreach (BufferElement element in layout.Elements) {
                GL.EnableVertexAttribArray(element.Index);
                GL.VertexAttribPointer(element.Index, element.ComponentCount, element.AttribType, element.Normalized, layout.Stride, element.Offset);
                GLHelper.CheckGLError("SetAttribPointer");
            }
        }
        public void Dispose() {
            GL.DeleteVertexArray(this.vaoID);
        }
    }
}