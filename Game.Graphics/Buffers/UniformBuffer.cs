using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public struct UniformBuffer : IDisposable {
        public int Size { get; set; }
        public readonly int uboID { get; }
        public UniformBuffer(int size, int binding) {
            this.Size = size;
            this.uboID = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.UniformBuffer, this.uboID);
            GL.NamedBufferData(this.uboID, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, binding, this.uboID);

        }
        public void SetData(IntPtr data, int size, int offset=0) {
            GL.NamedBufferSubData(this.uboID, (IntPtr)offset, size, data);
        }
        
        public void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.uboID);
        }

        public void Unbind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public void Dispose() {
            GL.DeleteBuffer(this.uboID);
        }
    }
}