using OpenTK.Graphics.OpenGL4;
using System;

namespace Game.Graphics {
    public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged {
        public int Size { get; set; }
        public int bufferID { get; private set; }
        public BufferTarget Type { get; private set;}
        public bool IsImmutable { get; private set; }
        public BufferObject(int size, BufferTarget type, in TDataType[] data=null, int uniform_binding=0) {
            this.Size = size;
            this.bufferID = GL.GenBuffer();
            this.Type = type;
            this.IsImmutable = data != null;

            GL.BindBuffer(this.Type, this.bufferID);

            if (this.Type == BufferTarget.UniformBuffer) {
                GL.NamedBufferData(this.bufferID, size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, uniform_binding, this.bufferID);
            } else {
                GL.BufferData(this.Type, this.Size, data, this.IsImmutable ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw);
            }
        }
        public void SetNamedSubData(IntPtr data, int size, int offset=0) {
            GL.NamedBufferSubData(this.bufferID, (IntPtr)offset, size, data);
        }
        public void SetSubData(IntPtr data, int sizeInBytes) {
            GL.BufferSubData(this.Type, (IntPtr)0, sizeInBytes, data);
        }
        public void Bind() {
            GL.BindBuffer(this.Type, this.bufferID);
        }
        public void Dispose() {
            GL.DeleteBuffer(this.bufferID);
        }
    }
}