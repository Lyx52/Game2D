using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Game.Utils;
using System.Runtime.InteropServices;
using System.Buffers;
namespace Game.Graphics {
    public enum QuadMode {
        CENTER,
        CORNER
    }
    public enum RenderLayer {
        BACKGROUND,
        LAYER_1,
        LAYER_2,
        LAYER_3,
        LAYER_4,
        LAYER_5,
        LAYER_6
    }
    public struct DrawQuad2D : IComparable<DrawQuad2D> {
        public Vector2 Position;
        public Vector2 Size;
        public Texture Texture;
        public Vector2[] TexCoords;
        public Vector4 Color;
        public float Rotation;
        public int Layer;
        public DrawQuad2D(in Vector2 position, in Vector2 size, in SpriteSheet spriteSheet, in Vector2i spritePos, in float rotation=0, in RenderLayer layer=RenderLayer.BACKGROUND) : this(position, size, spriteSheet.SpriteTexture, spriteSheet.GetSubSprite(spritePos), Vector4.One, rotation:rotation, layer:layer){}
        public DrawQuad2D(in Vector2 position, in Vector2 size, in Texture texture, in Vector2[] textureUV, in float rotation=0, in RenderLayer layer=RenderLayer.BACKGROUND) : this(position, size, texture, textureUV, Vector4.One, rotation:rotation, layer:layer){}
        public DrawQuad2D(in Vector2 position, in Vector2 size, in Texture texture, in Vector2[] textureUV, in Vector4 color, in float rotation=0, in RenderLayer layer=RenderLayer.BACKGROUND) {
            this.Position = position;
            this.Size = size;
            this.Texture = texture;
            this.TexCoords = textureUV;
            this.Color = color;
            this.Rotation = rotation;
            this.Layer = (int)layer;
        }
        public int CompareTo(DrawQuad2D other_quad) {
            return this.Layer.CompareTo(other_quad.Layer);
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct QuadVertex {
        [FieldOffset(0)] public Vector2 Position;
        [FieldOffset(8)] public Vector4 Color;
        [FieldOffset(24)] public Vector2 TexCoord;
        [FieldOffset(32)] public float TexIndex;
        [FieldOffset(36)] public float Radiuss;

        // If radiuss is 0 then shape will be default quad
        public QuadVertex(in Vector2 position, in Vector4 color, in Vector2 texCoord, in float texIndex) : this(position, color, texCoord, texIndex, 0) {}
        public QuadVertex(in Vector2 position, in Vector4 color, in Vector2 texCoord, in float texIndex, in float rad) {
            this.Color = color;
            this.TexIndex = texIndex;
            this.TexCoord = texCoord;
            this.Position = position;
            this.Radiuss = rad;
        }
    }
    public struct RenderStorage : IDisposable {
        public Texture[] TextureUnits { get; set; }
        public int TextureUnitIndex { get; set; }
        public QuadVertex[] VertexArray { get; set; }
        public GCHandle VertexArrayHandle { get; private set; }
        public int VertexCount { get; set; }
        public int Indices { get; set; }
        public readonly int MAX_TEXTURE_UNITS { get; }
        public readonly int MAX_VERTICES { get; }
        public readonly int MAX_QUADS { get; }
        public readonly int MAX_INDICES { get; }
        public int Flushes { get; set; }
        public int PrevFlushes { get; set; }
        private readonly Vector4[] CenteredQuad { get; }
        private readonly Vector4[] CornerQuad { get; }
        public readonly Vector2[] DefaultTextureUV { get; }
        public List<DrawQuad2D> DispatchedQuads { get; set; }
        public SortedList<string, Texture> Textures;
        public RenderStorage(int bufferSize) {
            this.TextureUnitIndex = 0;
            this.VertexCount = 0;
            this.Indices = 0;
            this.Flushes = 0;
            this.PrevFlushes = 0;
            this.MAX_TEXTURE_UNITS = 32;
            unsafe {
                this.MAX_QUADS = (int)((bufferSize / sizeof(QuadVertex)) / 4);
            }
            this.MAX_VERTICES = this.MAX_QUADS * 4;
            this.MAX_INDICES = this.MAX_QUADS * 6;

            this.CenteredQuad = new Vector4[4] {
                new Vector4( 0.5f,  0.5f, 0.0f, 1.0f),
                new Vector4( 0.5f, -0.5f, 0.0f, 1.0f),
                new Vector4(-0.5f, -0.5f, 0.0f, 1.0f),
                new Vector4(-0.5f,  0.5f, 0.0f, 1.0f)
            };
            
            this.CornerQuad = new Vector4[4] {
                new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(0.0f, 1.0f, 0.0f, 1.0f)
            };

            this.DefaultTextureUV = Renderer.DefaultUVCoords;

            // Create quad vertex array pin it to get its pointer
            this.VertexArray = new QuadVertex[MAX_VERTICES];
            this.VertexArrayHandle = GCHandle.Alloc(this.VertexArray, GCHandleType.Pinned);

            this.DispatchedQuads = new List<DrawQuad2D>();
            this.TextureUnits = new Texture[MAX_TEXTURE_UNITS];
            
            this.Textures = new SortedList<string, Texture>();
            this.Textures.Add("default", Texture.WhiteTexture);
            GC.KeepAlive(this.Textures);

            // Fill texture units with the default texture(Must be the original one with the original textureID!)
            for (int i = 0; i < MAX_TEXTURE_UNITS; i ++) {
                this.TextureUnits[i] = this.Textures["default"];
            }
            GC.KeepAlive(this);
        }
        public void Reset() {
            this.VertexCount = 0;
            this.Indices = 0;
            this.TextureUnitIndex = 0;
        }
        public Vector4[] GetVertexQuad(QuadMode mode) {
            switch(mode) {
                case QuadMode.CENTER: return this.CenteredQuad;
                case QuadMode.CORNER: return this.CornerQuad;
                default: return this.CenteredQuad;
            }
        }
        public bool IsOverflow() {
            return (this.VertexCount + 4) > this.MAX_VERTICES || this.TextureUnitIndex >= this.MAX_TEXTURE_UNITS;
        }
        public void Dispose() {
            GameHandler.Logger.Warn("Disposing render storage! Is this intentional?");
            this.VertexArrayHandle.Free();
            foreach (Texture tex in this.Textures.Values) {
                tex.Dispose();
            }
        }
    }
    public class Renderer {
        private GLState RendererState;
        private RenderStorage Storage;
        private ShaderProgram TextureShader;
        private VertexArray<uint, float> VertexArray;
        private BufferObject<float> VertexBuffer;
        private BufferObject<uint> IndexBuffer;
        private BufferObject<float> CameraBuffer;
        public Vector2 DrawSize { get; set; }
        public static Vector2[] DefaultUVCoords;
        
        public OrthoCamera RenderCamera;
        public int PrevVertexCount = 0;
        private int CurrentVertexCount = 0;
        public Renderer(int bufferSize, Vector2i drawSize) : this(bufferSize, drawSize.X, drawSize.Y) {}
        public Renderer(int bufferSize, int width, int height) {
            DefaultUVCoords = new Vector2[4] {
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f)
            };
            this.DrawSize = new Vector2(width, height);
            this.RendererState = new GLState();
            this.RendererState.SetClearColor(0.0f, 0.0f, 1.0f, 1.0f);
            this.RendererState.EnableAlpha();
            this.RendererState.EnableScissorTest();

            this.TextureShader = new ShaderProgram("./res/shaders/TextureShader.vert", "./res/shaders/TextureShader.frag");
            this.TextureShader.Bind();

            // Setup vertex buffer/array
            this.VertexArray = new VertexArray<uint, float>();

            BufferLayout VertexLayout = new BufferLayout(new List<BufferElement>() {
                new BufferElement("position", ElementType.Float2, 0),
                new BufferElement("color", ElementType.Float4, 1),
                new BufferElement("uvCoord", ElementType.Float2, 2),
                new BufferElement("texIndex", ElementType.Float, 3),
                new BufferElement("rad", ElementType.Float, 4)
            });

            this.Storage = new RenderStorage(bufferSize);
            this.VertexBuffer = new BufferObject<float>(VertexLayout.Size * this.Storage.MAX_VERTICES, BufferTarget.ArrayBuffer);
            this.VertexArray.AddVertexBuffer(this.VertexBuffer, VertexLayout);

            // Setup buffer of indices
            uint[] quadIndices = new uint[this.Storage.MAX_INDICES];
            uint offset = 0;

            for (int i = 0; i < this.Storage.MAX_INDICES; i += 6) {
                quadIndices[i] =     (uint)offset;
                quadIndices[i + 1] = (uint)offset + 1;
                quadIndices[i + 2] = (uint)offset + 3;
                quadIndices[i + 3] = (uint)offset + 1;
                quadIndices[i + 4] = (uint)offset + 2;
                quadIndices[i + 5] = (uint)offset + 3;
                offset += 4;
            }
            this.IndexBuffer = new BufferObject<uint>(sizeof(uint) * this.Storage.MAX_INDICES, BufferTarget.ElementArrayBuffer, data: quadIndices);
            this.VertexArray.Bind();
            this.VertexArray.SetIndexBuffer(this.IndexBuffer);

            unsafe {
                this.CameraBuffer = new BufferObject<float>(sizeof(Matrix4), BufferTarget.UniformBuffer, uniform_binding: 1);
            }
            // Init renderer camera camera draw size is window width divided by aspect ratio
            this.RenderCamera = new OrthoCamera(this.DrawSize);

            // Collect garbage, for some reason garbage collector collects our buffers
            GC.Collect();
        }
        public Matrix4 CreateTransformMatrix(Vector2 position, Vector2 size, float rotation) {
            Matrix4 _translation = Matrix4.CreateTranslation(position.X, position.Y, 0);
            Matrix4 _rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians((float)rotation));
            Matrix4 _scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
            return Matrix4.Mult(Matrix4.Mult(_scale, _rotation), _translation);
        }
        public void DispatchQuad(DrawQuad2D quad) {
            this.Storage.DispatchedQuads.Add(quad);
        }
        public void GenerateQuadGeometry() {

            // We need to sort quads by their layer(We do this to avoid depth buffers)
            this.Storage.DispatchedQuads.Sort();
            foreach (DrawQuad2D quad in this.Storage.DispatchedQuads) {
                this.DrawQuad(quad);
            }
        }
        public void DrawQuad(DrawQuad2D quad) {
            if (this.Storage.IsOverflow()) {
                this.NextBatch();
            }
            int textureIndex = this.AddUniqueTexture(quad.Texture);
            Matrix4 transform = this.CreateTransformMatrix(quad.Position, quad.Size, quad.Rotation);
            for (int i = 0; i < 4; i++) {
                // This actually only check if its a center mode, if its any other mode it will always be corner
                Vector4 vertexPosition = (this.Storage.GetVertexQuad(RendererState.QuadRenderMode))[i] * transform;

                this.Storage.VertexArray[this.Storage.VertexCount++] = new QuadVertex(vertexPosition.Xy, quad.Color, quad.TexCoords[i], textureIndex);
                this.CurrentVertexCount++;
            }
            this.Storage.Indices += 6;
        }
        public void StartScene(in Vector2 cameraPosition) {
            GL.Scissor(0, 0, GameHandler.WindowSize.X, GameHandler.WindowSize.Y);
            unsafe {
                this.RenderCamera.Recalculate(cameraPosition);
                this.CameraBuffer.SetData(this.RenderCamera.GetViewProjection(), sizeof(Matrix4));
            }
            this.StartBatch();
        }
        public void EndScene() {
            this.GenerateQuadGeometry();
            this.Flush();
            this.Storage.PrevFlushes = this.Storage.Flushes;
            this.Storage.Flushes = 0;
            this.PrevVertexCount = this.CurrentVertexCount;
            this.CurrentVertexCount = 0;
            this.Storage.DispatchedQuads.Clear();
        }
        public void StartBatch() {
            this.Storage.Reset();
        }
        public void NextBatch() {
            this.Flush();
            this.StartBatch();
        }
        public void Flush() {
            if (this.Storage.VertexCount > 0) {
                this.Storage.Flushes++;
                
                // Upload vertex data
                this.VertexArray.Bind();
                this.VertexBuffer.Bind();
                this.TextureShader.Bind();
                unsafe {
                    this.VertexBuffer.SetData(this.Storage.VertexArrayHandle.AddrOfPinnedObject(), sizeof(QuadVertex) * this.Storage.VertexCount);
                }
                
                // Bind textures
                this.BindTextureUnits();
                this.DrawIndexed(PrimitiveType.Triangles);

                this.TextureShader.Unbind();
            }
        }
        public void OnResize(ResizeEventArgs args) {
            GameHandler.WindowSize = args.Size;
            this.DrawSize = Vector2i.Divide(args.Size, GameHandler.AspectRatio);
            GL.Viewport(0, 0, args.Size.X, args.Size.Y);   
        }
        public void DrawIndexed(PrimitiveType type) {
            GL.DrawElements(type, this.Storage.Indices, DrawElementsType.UnsignedInt, 0);
        }
        public void DrawPrimative(PrimitiveType type) {
            GL.DrawArrays(type, 0, this.Storage.VertexCount);
        }
        private int AddUniqueTexture(Texture texture) {
            if (ArrayUtils.IndexOf<Texture>(this.Storage.TextureUnits, texture) == -1) {
                this.Storage.TextureUnits[this.Storage.TextureUnitIndex++] = texture;
            } else if (!this.Storage.TextureUnits[this.Storage.TextureUnitIndex].Equals(this.GetTexture("default"))) {
                this.Storage.TextureUnitIndex++;
            }
            return GetTextureUnit(texture);
        }
        public int GetTextureUnit(Texture texture) {
            for (int i = 0; i < this.Storage.TextureUnitIndex; i++) {
                if (this.Storage.TextureUnits[i].Equals(texture)) {
                    return i;
                }
            }
            return 0;
        } 
        public void BindTextureUnits() {
            // Bind only available texture units
            for (int i = 0; i < this.Storage.TextureUnitIndex; i++) {
                this.Storage.TextureUnits[i].BindToUnit(i);
            }
        }
        public int TotalFlushes {
            get { return this.Storage.PrevFlushes; }
        }
        public int TotalVertices {
            get { return this.TotalFlushes * this.Storage.MAX_VERTICES; }
        }
        public void Dispose() {
            this.RenderCamera.Dispose();
            this.Storage.Dispose();
        }
        public void AddTexture(string name, Texture tex) {
            if (!this.Storage.Textures.TryAdd(name, tex)) {
                GameHandler.Logger.Error($"Couldn't add {name} texture");
            }
            GC.Collect();
        }
        public void LoadTexture(string name, string location) {
            this.AddTexture(name, Texture.LoadTexture(location));
        }
        public Texture GetTexture(string name) {
            if (this.Storage.Textures.TryGetValue(name, out Texture tex)) {
                return tex;
            } else {
                GameHandler.Logger.Error($"Texture {name} doesn't exist!");
                return this.Storage.Textures["default"];
            }
        }
    }
}