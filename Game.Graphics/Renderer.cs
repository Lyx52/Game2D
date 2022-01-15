using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Game.Utils;
using System.Runtime.InteropServices;

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
    public struct Vertex {
        [FieldOffset(0)] public Vector2 Position;
        [FieldOffset(8)] public Vector4 Color;
        [FieldOffset(24)] public Vector2 TexCoord;
        [FieldOffset(32)] public float TexIndex;
        public Vertex(in Vector2 position, in Vector4 color, in Vector2 texCoord, in float texIndex) {
            this.Color = color;
            this.TexIndex = texIndex;
            this.TexCoord = texCoord;
            this.Position = position;
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct SimpleVertex {
        [FieldOffset(0)] public Vector2 Position;
        [FieldOffset(8)] public Vector4 Color;
        public SimpleVertex(in Vector2 position, in Vector4 color) {
            this.Color = color;
            this.Position = position;
        }
    }
    public struct RenderStats {
        public int TotalFlushCount { get; set; }
        public int CurrentVertexCount { get; set; }
        private int TotalVertexCount { get; set; }
        public int CurrentQuadCount { get; set; }
        private int TotalQuadCount { get; set; }
        public void Reset() {
            this.TotalQuadCount = this.CurrentQuadCount;
            this.TotalVertexCount = this.CurrentVertexCount;
            this.CurrentQuadCount = 0;
            this.CurrentVertexCount = 0;
        }
        public override string ToString()
        {
            return $"Flushes: {this.TotalFlushCount}, Vertices: {this.TotalVertexCount}, Quads: {this.TotalQuadCount}";
        }
    }
    public unsafe struct RenderStorage : IDisposable {
        public Texture[] TextureUnits { get; set; }
        public int TextureUnitIndex { get; set; }
        public readonly int MAX_TEXTURE_UNITS { get; }
        public readonly int MAX_VERTICES { get; }
        public readonly int MAX_QUADS { get; }
        public readonly int MAX_INDICES { get; }
        public readonly Vector4[] CenteredQuad { get; }
        public readonly Vector4[] CornerQuad { get; }
        public List<DrawQuad2D> DispatchedQuads { get; set; }
        public SortedList<string, Texture> Textures;
        public RenderStats Stats;
        public RenderStorage(int bufferSize) {
            this.TextureUnitIndex = 0;
            this.Stats = new RenderStats();
            this.MAX_TEXTURE_UNITS = 32;
            this.MAX_QUADS = (int)((bufferSize / sizeof(Vertex)) / 4);
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
            this.Stats.Reset();
            this.TextureUnitIndex = 0;
        }
        public bool IsOverflow() {
            return (this.TextureUnitIndex + 1) >= this.MAX_TEXTURE_UNITS;
        }
        public void Dispose() {
            GameHandler.Logger.Warn("Disposing render storage! Is this intentional?");
            foreach (Texture tex in this.Textures.Values) {
                tex.Dispose();
            }
        }
    }
    public class Renderer {
        private GLState RendererState;
        private RenderStorage Storage;
        private ShaderProgram TextureShader;
        private VertexArray<uint, Vertex> QuadVertexArray;
        private BufferObject<float> CameraBuffer;
        public static Vector2[] DefaultUVCoords;
        public OrthoCamera RenderCamera;
        public TextHandler TextRenderer;
        public Renderer(int bufferSize, Vector2i drawSize) : this(bufferSize, drawSize.X, drawSize.Y) {}
        public Renderer(int bufferSize, int width, int height) {
            DefaultUVCoords = new Vector2[4] {
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f)
            };
            this.RendererState = new GLState();
            this.RendererState.SetClearColor(0.0f, 0.0f, 1.0f, 1.0f);
            this.RendererState.AlphaBlend.Enable();
            this.RendererState.ScissorTest.Enable();
            this.TextureShader = new ShaderProgram("./res/shaders/TextureShader.vert", "./res/shaders/TextureShader.frag");
            this.TextureShader.Bind();
            this.Storage = new RenderStorage(bufferSize);

            VertexLayout VertexLayout = new VertexLayout(new List<VertexElement>() {
                new VertexElement("position", ElementType.Vector2f, 0),
                new VertexElement("color", ElementType.Vector4f, 1),
                new VertexElement("uvCoord", ElementType.Vector2f, 2),
                new VertexElement("texIndex", ElementType.Float, 3)
            });

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

            // Setup vertex buffer/array
            this.QuadVertexArray = new VertexArray<uint, Vertex>(VertexLayout, this.Storage.MAX_INDICES, this.Storage.MAX_VERTICES, indices:quadIndices);
            this.TextRenderer = new TextHandler();
            TextHandler.ReadFont("./res/font/arial.ttf");
            GameHandler.Logger.Debug($"CurrentFont: {TextHandler.CurrentFace.MarshalFamilyName()}");
            unsafe {
                this.CameraBuffer = new BufferObject<float>(4 * 4, BufferTarget.UniformBuffer, uniform_binding: 1);
            }

            // Init renderer camera camera draw size is window width divided by aspect ratio
            this.RenderCamera = new OrthoCamera(-(width / 2), (width / 2), -(height / 2), (height / 2));

            // Collect garbage, for some reason garbage collector collects our buffers
            GC.Collect();
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
            if (this.QuadVertexArray.IsOverflow() || this.Storage.IsOverflow()) {
                this.NextBatch();
            }
            int textureIndex = this.AddUniqueTexture(quad.Texture);
            Matrix4 transform = MathUtils.CreateTransform(quad.Position, quad.Size, quad.Rotation);
            for (int i = 0; i < 4; i++) {
                // This actually only check if its a center mode, if its any other mode it will always be corner
                Vector4 vertexPosition = this.DefaultQuad[i] * transform;
                this.QuadVertexArray.AppendVertex(new Vertex(vertexPosition.Xy, quad.Color, quad.TexCoords[i], textureIndex));
                this.Storage.Stats.CurrentVertexCount++;
            }

            this.QuadVertexArray.IndicesCount += 6;
            this.Storage.Stats.CurrentQuadCount++;
        }
        public void StartScene(in Vector2 cameraPosition) {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Scissor(0, 0, GameHandler.WindowSize.X, GameHandler.WindowSize.Y);

            this.Storage.Stats.TotalFlushCount = 0;
            
            unsafe {
                this.RenderCamera.Recalculate(cameraPosition);
                this.CameraBuffer.SetSubData(this.RenderCamera.ViewProjection, sizeof(Matrix4));
            }
            
            this.StartBatch();
            // this.TextVertexArray.Bind();
            // this.TextShader.Bind();
            // this.DrawIndexed(PrimitiveType.Triangles, 6);
        }
        public void EndScene() {
            this.GenerateQuadGeometry();
            this.Flush();
            GameHandler.Profiler.StartSection("TextRendering");
            this.TextRenderer.RenderText("Test", new Vector2(20f, 20f), 2.0F, new Vector3(0.5F, 0.8F, 0.2F));
            this.TextRenderer.RenderText("Test2", new Vector2(100f, 40f), 2.0F, new Vector3(0.5F, 0.8F, 0.2F), fontSize:24);
            GameHandler.Profiler.EndSection("TextRendering");
            //this.TextRenderer.RenderBuffers();
            this.Storage.DispatchedQuads.Clear();
        }
        public void StartBatch() {
            this.Storage.Reset();
            this.QuadVertexArray.Reset();
        }
        public void NextBatch() {
            this.Flush();
            this.StartBatch();
        }
        public void Flush() {
            if (this.QuadVertexArray.VertexCount > 0) {
                this.Storage.Stats.TotalFlushCount++;
                
                // Upload vertex data
                this.QuadVertexArray.Bind();
                this.QuadVertexArray.Flush();

                this.TextureShader.Bind();

                // Bind textures
                this.BindTextureUnits();
                DrawIndexed(PrimitiveType.Triangles, this.QuadVertexArray.IndicesCount);
            }
        }
        public void OnResize(ResizeEventArgs args) {
            GameHandler.WindowSize = args.Size;
            GL.Viewport(0, 0, args.Size.X, args.Size.Y);   
        }
        public static void DrawIndexed(PrimitiveType type, int indices) {
            GL.DrawElements(type, indices, DrawElementsType.UnsignedInt, 0);
        }
        public static void DrawPrimative(PrimitiveType type, int vertices) {
            GL.DrawArrays(type, 0, vertices);
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
        public void Dispose() {
            this.RenderCamera.Dispose();
            this.TextRenderer.Dispose();
            this.Storage.Dispose();

        }
        public void AddTexture(string name, Texture tex) {
            if (!this.Storage.Textures.TryAdd(name, tex)) {
                GameHandler.Logger.Error($"Couldn't add {name} texture");
            }
            GC.Collect();
        }
        public void LoadTexture(string name, string location) {
            this.AddTexture(name, Texture.Load(location));
        }
        public Vector4[] DefaultQuad {
            get {
                switch(RendererState.QuadRenderMode) {
                    case QuadMode.CENTER: return this.Storage.CenteredQuad;
                    case QuadMode.CORNER: return this.Storage.CornerQuad;
                    default: return this.Storage.CenteredQuad;
                }
            }
        }
        public Texture GetTexture(string name) {
            if (this.Storage.Textures.TryGetValue(name, out Texture tex)) {
                return tex;
            } else {
                GameHandler.Logger.Error($"Texture {name} doesn't exist!");
                return this.Storage.Textures["default"];
            }
        }
        public string GetStats() {
            return $"Mem: {Math.Round((double)System.GC.GetTotalMemory(false) / (1000 * 1000), 2)}Mb, {this.Storage.Stats.ToString()}";
        }
    }
}