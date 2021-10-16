using System.Collections.Generic;

namespace Game.Graphics {
    public struct BufferLayout {
        public List<BufferElement> Elements { get; }
        public int Offset { get; set; }
        public int Stride { get; set; } 
        public int Size { get; set; }
        public int Components { get; set; }
        public BufferLayout (List<BufferElement> elements) {
            this.Elements = elements;
            this.Offset = 0;
            this.Stride = 0;
            this.Components = 0;
            this.Size = 0;
            if (this.IsValid()) {
                this.CalculateLayout();
            }
        }
        public bool IsValid() {
            return this.Elements != null && this.Elements.Count > 0;
        }
        private void CalculateLayout() {
            for (int i = 0; i < this.Elements.Count; i++) {
                BufferElement element = this.Elements[i];
                
                element.Offset = this.Offset;
                this.Offset += element.Size;
                this.Stride += element.Size;
                this.Components += element.ComponentCount;
                this.Elements[i] = element;
            }
            this.Size = this.Offset;
        }
    }
}