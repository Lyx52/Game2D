using System.Collections.Generic;
using Game.Utils;

namespace Game.Graphics {
    public struct VertexLayout {
        public List<VertexElement> Elements { get; }
        public int Offset { get; private set; }
        public int Stride { get; private set; } 
        public int Size { get; private set; }
        public int Components { get; private set; }
        public VertexLayout (List<VertexElement> elements) {
            this.Elements = elements;
            this.Offset = 0;
            this.Stride = 0;
            this.Components = 0;
            this.Size = 0;
            
            Logger.Assert(elements != null && elements.Count > 0, "Invalid vertex element list passed!");
            this.CalculateLayout();
        }
        private void CalculateLayout() {
            for (int i = 0; i < this.Elements.Count; i++) {
                VertexElement element = this.Elements[i];
                element.Offset = this.Offset;
                this.Offset += element.Size;
                this.Stride += element.Size;
                this.Components += element.Components;
                this.Elements[i] = element;
            }
            this.Size = this.Offset;
        }
    }
}