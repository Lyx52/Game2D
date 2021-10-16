using System;
using OpenTK.Graphics.OpenGL4;
using System.Text.RegularExpressions;

namespace Game.Graphics {
    public enum ElementType : int {
        NoType,
        Float,
        Float2,
        Float3,
        Float4,
        Mat3,
        Mat4,
        Int,
        Int2,
        Int3,
        Int4,
        Bool
    }
    public enum ElementTypeComponents : int {
        NoType = 0,
        Float = 1,
        Float2 = 2,
        Float3 = 3,
        Float4 = 4,
        Mat3 = 3,
        Mat4 = 4,
        Int = 1,
        Int2 = 2,
        Int3 = 3,
        Int4 = 4,
        Bool = 1
    }
    public enum ElementTypeSize : int {
        NoType = 0,
        Float = 4,
        Float2 = 8,
        Float3 = 12,
        Float4 = 16,
        Mat3 = 36,
        Mat4 = 64,
        Int = 4,
        Int2 = 8,
        Int3 = 12,
        Int4 = 16,
        Bool = 1
    }

    public struct BufferElement {
        public readonly string Name { get; }
        public readonly ElementType Type { get; }
        public readonly int Index { get; }
        public int Offset { get; set; }
        public readonly bool Normalized { get; }
        
        public BufferElement(string name, ElementType type, int layoutIndex, bool normalized=false) {
            this.Name = name;
            this.Type = type;
            this.Offset = 0;
            this.Index = layoutIndex;
            this.Normalized = normalized;
        }
        public string TypeString {
            get {
                return this.Type.ToString();
            }
        }
        public int Size {
            get {
                return (int)Enum.Parse(typeof(ElementTypeSize), this.TypeString);    
            }
        }
        public int ComponentCount {
            get {
                return (int)Enum.Parse(typeof(ElementTypeComponents), this.TypeString);
            }
        }
        public VertexAttribPointerType AttribType {
            get {
                switch (Regex.Replace(this.TypeString, @"[\d-]", string.Empty)) {
                    case "Float": return VertexAttribPointerType.Float;
                    case "Mat": return VertexAttribPointerType.Float;
                    case "Int": return VertexAttribPointerType.Int;
                    case "Bool": return VertexAttribPointerType.Int;
                    default: return VertexAttribPointerType.Float;
                }    
            }
        }
    }
}