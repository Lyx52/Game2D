using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;

namespace Game.Graphics {
    public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged {
        public int Size { get; set; }
        public int bufferID { get; private set; }
        public BufferTarget Type { get; private set;}
        public bool IsImmutable { get; private set; }
        private TDataType[] BufferData { get; set; }
        public GCHandle BufferDataHandle { get; private set; }
        public int CurrentElementCount { get; set; }
        public unsafe BufferObject(int elementCount, BufferTarget type, in TDataType[] data=null, int uniform_binding=0) {
            this.Size = sizeof(TDataType) *  elementCount;
            this.bufferID = GL.GenBuffer();
            this.Type = type;
            this.CurrentElementCount = 0;
            this.IsImmutable = data != null;
            GL.BindBuffer(this.Type, this.bufferID);

            if (this.Type == BufferTarget.UniformBuffer) {
                GL.NamedBufferData(this.bufferID, this.Size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, uniform_binding, this.bufferID);
            } else {
                GL.BufferData(this.Type, this.Size, data, this.IsImmutable ? BufferUsageHint.StreamDraw : BufferUsageHint.StaticDraw);
                this.BufferData = new TDataType[elementCount];
                this.BufferDataHandle = GCHandle.Alloc(this.BufferData, GCHandleType.Pinned);
            }
            
        }
        public unsafe void Flush() {
            this.SetSubData(this.BufferDataHandle.AddrOfPinnedObject(), sizeof(TDataType) * this.CurrentElementCount);
        }
        public void SetNamedSubData(IntPtr data, int size, int offset=0) {
            GL.NamedBufferSubData(this.bufferID, (IntPtr)offset, size, data);
        }
        public void SetSubData(IntPtr data, int sizeInBytes) {
            GL.BufferSubData(this.Type, (IntPtr)0, sizeInBytes, data);
        }
        public void AppendElement(in TDataType data) {
            if (this.IsImmutable)
                GameHandler.Logger.Critical($"Trying to modify immutable {this.Type} buffer!");
            this.BufferData[this.CurrentElementCount++] = data;
        }
        public bool IsOverflow(int elementGap) {
            return (this.CurrentElementCount + elementGap) >= this.BufferData.Length;
        }
        public void Bind() {
            GL.BindBuffer(this.Type, this.bufferID);
        }
        public void Dispose() {
            GL.DeleteBuffer(this.bufferID);
            if (this.BufferDataHandle.IsAllocated)
                this.BufferDataHandle.Free();
        }
    }
}