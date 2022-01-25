using OpenTK.Graphics.OpenGL4;
using Game.Utils;

namespace Game.Graphics {
    public enum ElementType : int {
        NoType,
        Float,
        Vector2f,
        Vector3f,
        Vector4f,
        Matrix3x3f,
        Matrix4x4f,
        Int,
        Vector2i,
        Vector3i,
        Vector4i,
        Bool
    }
    public struct VertexElement {
        public readonly string Name { get; }
        // Attribute index
        public readonly int Index { get; }
        // Attribute component or how many floats/ints etc element contains
        public readonly int Components { get; }
        // Attribute type
        public readonly VertexAttribPointerType Type { get; }
        // Is value normalized
        public readonly bool Normalized { get; }
        // Attribute element size in bytes
        public readonly int Size { get; }
        // Value offset in a vertex
        public int Offset { get; set; }
        public VertexElement(string name, ElementType type, int layoutIndex, bool normalized=false) {
            this.Name = name;
            this.Index = layoutIndex;
            this.Components = VertexElement.GetElementComponents(type);
            this.Type = VertexElement.GetElementGLType(type);
            this.Size = VertexElement.GetElementSize(type);
            this.Normalized = normalized;
            this.Offset = 0;
        }
        public static VertexAttribPointerType GetElementGLType(ElementType type) {
            switch(type) {
                case ElementType.NoType: return default;
                case ElementType.Float: return VertexAttribPointerType.Float;
                case ElementType.Vector2f: return VertexAttribPointerType.Float;
                case ElementType.Vector3f: return VertexAttribPointerType.Float;
                case ElementType.Vector4f: return VertexAttribPointerType.Float;
                case ElementType.Matrix3x3f: return VertexAttribPointerType.Float;
                case ElementType.Matrix4x4f: return VertexAttribPointerType.Float;
                case ElementType.Int: return VertexAttribPointerType.Int;
                case ElementType.Vector2i: return VertexAttribPointerType.Int;
                case ElementType.Vector3i: return VertexAttribPointerType.Int;
                case ElementType.Vector4i: return VertexAttribPointerType.Int;
                case ElementType.Bool: return VertexAttribPointerType.Int;
                default: {
                    Logger.Error("Unkown element type!");
                    return 0;
                }
            }    
        }
        public static int GetElementComponents(ElementType type) {
            switch(type) {
                case ElementType.NoType: return 0;
                case ElementType.Float: return 1;
                case ElementType.Vector2f: return 2;
                case ElementType.Vector3f: return 3;
                case ElementType.Vector4f: return 4;
                case ElementType.Matrix3x3f: return 12;
                case ElementType.Matrix4x4f: return 16;
                case ElementType.Int: return 1;
                case ElementType.Vector2i: return 2;
                case ElementType.Vector3i: return 3;
                case ElementType.Vector4i: return 4;
                case ElementType.Bool: return 1;
                default: {
                    Logger.Error("Unkown element type!");
                    return 0;
                }
            }
        }
        public static int GetElementSize(ElementType type) {
            switch(type) {
                case ElementType.NoType: return 0;
                case ElementType.Float: return 4;
                case ElementType.Vector2f: return 8;
                case ElementType.Vector3f: return 12;
                case ElementType.Vector4f: return 16;
                case ElementType.Matrix3x3f: return 36;
                case ElementType.Matrix4x4f: return 64;
                case ElementType.Int: return 4;
                case ElementType.Vector2i: return 8;
                case ElementType.Vector3i: return 12;
                case ElementType.Vector4i: return 16;
                case ElementType.Bool: return 1;
                default: {
                    Logger.Error("Unkown element type!");
                    return 0;
                }
            }
        }
    }
}