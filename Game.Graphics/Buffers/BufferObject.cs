using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public class BufferObject<T> : IDisposable where T : unmanaged {
        public int Size { get; set; }
        public int bufferID { get; private set; }
        public BufferTarget Type { get; private set;}
        public BufferObject(int size, BufferTarget type, T[] data=null, int uniform_binding=0) {
            this.Size = size;
            this.bufferID = GL.GenBuffer();
            this.Type = type;
            GL.BindBuffer(this.Type, this.bufferID);

            if (this.Type == BufferTarget.UniformBuffer) {
                GL.NamedBufferData(this.bufferID, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, uniform_binding, this.bufferID);
            } else {
                GL.BufferData(this.Type, this.Size, data, data == null ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
            }
        }
        public void SetNamedData(IntPtr data, int size, int offset=0) {
            GL.NamedBufferSubData(this.bufferID, (IntPtr)offset, size, data);
        }
        public void SetData(IntPtr data, int sizeInBytes) {
            GL.BufferSubData(this.Type, (IntPtr)0, sizeInBytes, data);
        }
        public void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.bufferID);
        }
        public void Dispose() {
            GL.DeleteBuffer(this.bufferID);
        }
    }
}