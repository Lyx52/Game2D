using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Game.Utils;
using Game.Entity;

namespace Game.Graphics {
    public enum QuadMode {
        CENTER,
        CORNER
    }
    public struct QuadVertex {
        public Vector2 Position;
        public Vector4 Color;
        public Vector2 TexCoord;
        public float TexIndex;
        public QuadVertex(Vector2 position, Vector4 color, Vector2 texCoord, float texIndex) {
            this.Color = color;
            this.TexIndex = texIndex;
            this.TexCoord = texCoord;
            this.Position = position;
        }
    }
    public struct RenderStorage {
        public Texture[] TextureUnits { get; set; }
        public int TextureUnitIndex { get; set; }
        public QuadVertex[] VertexArray { get; set; }
        public IntPtr VertexArrayPtr { get; set; }
        public int VertexCount { get; set; }
        public int Indices { get; set; }
        public Texture DefaultTexture { get; set; }
        public readonly int MAX_TEXTURE_UNITS { get; }
        public readonly int MAX_VERTICES { get; }
        public readonly int MAX_QUADS { get; }
        public readonly int MAX_INDICES { get; }
        public int Flushes { get; set; }
        public int PrevFlushes { get; set; }
        private readonly Vector4[] CenteredQuad { get; }
        private readonly Vector4[] CornerQuad { get; }
        public readonly Vector2[] DefaultTextureUV { get; }

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

            this.DefaultTextureUV = Renderer.GetDefaultUVCoords();
            this.VertexArray = new QuadVertex[MAX_VERTICES];
            this.VertexArrayPtr = IOUtils.GetObjectPtr(this.VertexArray); 

            this.TextureUnits = new Texture[MAX_TEXTURE_UNITS];
            this.DefaultTexture = Texture.WhiteTexture;

            for (int i = 0; i < MAX_TEXTURE_UNITS; i ++) {
                this.TextureUnits[i] = DefaultTexture;
            }
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
    }
    public class Renderer {

        public event Action<FrameEventArgs> OnEndScene;
        public event Action<Renderer> OnStartScene;
        public event Action<Renderer> OnRender;
        private GLState RendererState;
        private RenderStorage Storage;
        private ShaderProgram TextureShader;
        private UniformBuffer CameraBuffer;
        private VertexArray VertexArray;
        private VertexBuffer VertexBuffer;
        private BufferLayout VertexLayout;
        public Texture DIRT_TEXTURE;
        public Texture APPLE_TEXTURE;
        private IntPtr ViewProjectionMatrix;
        public Renderer(int bufferSize, int width, int height) {
            this.RendererState = new GLState();
            this.RendererState.SetClearColor(0.0f, 0.0f, 1.0f, 1.0f);
            this.RendererState.EnableAlpha();

            this.TextureShader = new ShaderProgram("./res/shaders/shader.vert", "./res/shaders/shader.frag");
            this.TextureShader.Bind();
            this.DIRT_TEXTURE = Texture.LoadTexture("./res/textures/grass.png");
            this.APPLE_TEXTURE = Texture.LoadTexture("./res/textures/apple.png");

            // Setup vertex buffer/array
            this.VertexArray = new VertexArray();
            this.VertexLayout = new BufferLayout(new List<BufferElement>() {
                new BufferElement("position", ElementType.Float2, 0),
                new BufferElement("color", ElementType.Float4, 1),
                new BufferElement("uvCoord", ElementType.Float2, 2),
                new BufferElement("texIndex", ElementType.Float, 3)    
            });

            this.Storage = new RenderStorage(bufferSize);
            this.VertexBuffer = new VertexBuffer(this.VertexLayout.Size * this.Storage.MAX_VERTICES);
            this.VertexBuffer.Layout = this.VertexLayout;
            this.VertexArray.AddVertexBuffer(this.VertexBuffer);

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
            this.VertexArray.SetIndexBuffer(new IndexBuffer(sizeof(uint) * this.Storage.MAX_INDICES, data:quadIndices));
            unsafe {
                this.CameraBuffer = new UniformBuffer(sizeof(Matrix4), 1);
                this.ViewProjectionMatrix = IOUtils.GetObjectPtr(Matrix4.Zero);
            }
            // Bind events
            this.OnEndScene += EndScene;
            this.OnStartScene += StartScene;

            // Display GL info
            GLHelper.DisplayGLInfo();
        }
        public Matrix4 CreateTransformMatrix(Vector2 position, Vector2 size, float rotation) {
            Matrix4 _translation = Matrix4.CreateTranslation(position.X, position.Y, 0);
            Matrix4 _rotation = Matrix4.CreateRotationZ((float)MathUtils.ToRadians((float)rotation));
            Matrix4 _scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
            return Matrix4.Mult(Matrix4.Mult(_scale, _rotation), _translation);
        }
        public void DrawQuad(Vector2 position, Vector2 size, Texture texture, Vector2[] textureUV, Vector4 color, float rotation=0) {
            if (this.Storage.IsOverflow()) {
                this.NextBatch();
            }
            float textureIndex = this.AddUniqueTexture(texture);
            Matrix4 transform = this.CreateTransformMatrix(position, size, rotation);
            for (int i = 0; i < 4; i++) {
                // This actually only check if its a center mode, if its any other mode it will always be corner
                Vector4 vertexPosition = (this.Storage.GetVertexQuad(RendererState.QuadRenderMode))[i] * transform;
                this.Storage.VertexArray[this.Storage.VertexCount++] = new QuadVertex(vertexPosition.Xy, color, textureUV[i], textureIndex);
            }
            this.Storage.Indices += 6;
        }
        public void DrawQuad(Vector2 position, Vector2 size, Texture texture, float rotation=0) {
            this.DrawQuad(position, size, texture, this.Storage.DefaultTextureUV, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), rotation:rotation);
        }
        public void SetViewProjection(Matrix4 viewProjection) {
            this.ViewProjectionMatrix = IOUtils.GetObjectPtr(viewProjection);
        }
        private void StartScene(Renderer renderer) {
            unsafe {
                this.CameraBuffer.SetData(this.ViewProjectionMatrix, sizeof(Matrix4));
            }
            this.StartBatch();
        }
        private void EndScene(FrameEventArgs args) {
            this.Flush();
            this.Storage.PrevFlushes = this.Storage.Flushes;
            this.Storage.Flushes = 0;
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
                this.VertexBuffer.Bind();
                unsafe {
                    this.VertexBuffer.SetDataQuadVertex(this.Storage.VertexArrayPtr, sizeof(QuadVertex) * this.Storage.VertexCount);
                }
                this.VertexArray.Bind();

                // Bind textures
                this.BindTextureUnits();
                
                this.DrawIndexed(PrimitiveType.Triangles);
                this.VertexArray.Unbind();
                this.VertexBuffer.Unbind();
            }
        }
        public void OnRenderFrame(FrameEventArgs args) {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            OnStartScene?.Invoke(this);
            OnRender?.Invoke(this);
            OnEndScene?.Invoke(args);
        }
        public void OnResize(ResizeEventArgs args) {
            GL.Viewport(0, 0, args.Size.X, args.Size.Y);   
        }
        public void DrawIndexed(PrimitiveType type) {
            GL.DrawElements(type, this.Storage.Indices, DrawElementsType.UnsignedInt, 0);
        }
        public void DrawPrimative(PrimitiveType type) {
            GL.DrawArrays(type, 0, this.Storage.VertexCount);
        }
        public int AddUniqueTexture(Texture texture) {
            if (Array.IndexOf(this.Storage.TextureUnits, texture) == -1) {
                this.Storage.TextureUnits[this.Storage.TextureUnitIndex++] = texture;
            } else if (this.Storage.TextureUnits[this.Storage.TextureUnitIndex] != this.Storage.DefaultTexture){
                this.Storage.TextureUnitIndex++;
            }
            return GetTextureUnit(texture);
        }
        public int GetTextureUnit(Texture texture) {
            for (int i = 0; i < this.Storage.TextureUnitIndex; i++) {
                if (this.Storage.TextureUnits[i] == texture) {
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
            get {
                return this.Storage.PrevFlushes;
            }
        }
        public static Vector2[] GetDefaultUVCoords() {
            return new Vector2[4] {
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f)
            };
        }
    }
}