using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;
using Game.Utils;

namespace Game.Graphics {
    public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged {
        public int Size { get; set; }
        public int bufferID { get; private set; }
        public BufferTarget Type { get; private set;}
        public bool IsImmutable { get; private set; }
        private TDataType[] BufferData { get; set; }
        public int CurrentElementCount { get; set; }
        public unsafe BufferObject(int elementCount, BufferTarget type, in TDataType[] data=null) {
            this.Size = sizeof(TDataType) *  elementCount;
            this.bufferID = GL.GenBuffer();
            this.Type = type;
            this.CurrentElementCount = 0;

            this.IsImmutable = data != null;
            GL.BindBuffer(this.Type, this.bufferID);
            GL.BufferData(this.Type, this.Size, data, this.IsImmutable ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
            this.BufferData = new TDataType[elementCount];      
        }
        public unsafe void Flush() {
            this.SetSubData(this.BufferData.AsSpan());
        }
        public void SetSubData(IntPtr ptr, int sizeInBytes) {
             GL.BufferSubData(this.Type, (IntPtr)0, sizeInBytes, ptr);    
        }
        public unsafe void SetSubData(ReadOnlySpan<TDataType> data) {
            fixed(TDataType* ptr = data) {
                GL.BufferSubData(this.Type, (IntPtr)0, sizeof(TDataType) * data.Length, (IntPtr)ptr);
            }
        }
        public unsafe void SetSubData(in TDataType[] data) {
            fixed(TDataType* ptr = data) {
                GL.BufferSubData(this.Type, (IntPtr)0, sizeof(TDataType) * data.Length, (IntPtr)ptr);
            }
        }
        public void AppendElement(in TDataType data) {
            if (this.IsImmutable)
                Logger.Critical($"Trying to modify immutable {this.Type} buffer!");
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
        }
    }
}