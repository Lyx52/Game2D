using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;

namespace Game.Graphics {
    public class VertexArray : IDisposable {
        public readonly int vaoID;
        private List<VertexBuffer> vertexBuffers;
        private IndexBuffer indexBuffer;

        public VertexArray() {
            this.vaoID = GL.GenVertexArray();
            this.vertexBuffers = new List<VertexBuffer>();
        }
        public void Bind() {
            GL.BindVertexArray(this.vaoID);
        }
        public void Unbind() {
            GL.BindVertexArray(0);
        }
        public void SetIndexBuffer(IndexBuffer buffer) {
            this.indexBuffer = buffer;
            this.indexBuffer.Bind();
        }
        public void AddVertexBuffer(VertexBuffer buffer) {
            if (!buffer.Layout.IsValid()) {
                GameHandler.Logger.Error("When adding a VertexBuffer it must have a valid layout!");
            }

            this.Bind();
            buffer.Bind();
            foreach (BufferElement element in buffer.Layout.Elements) {
                GL.EnableVertexAttribArray(element.Index);
                GL.VertexAttribPointer(element.Index, element.ComponentCount, element.AttribType, element.Normalized, buffer.Stride, element.Offset);
                GLHelper.CheckGLError("SetAttribPointer");
            }
            this.vertexBuffers.Add(buffer);
        }
        public void Dispose() {
            GL.DeleteVertexArray(this.vaoID);
        }
    }
}