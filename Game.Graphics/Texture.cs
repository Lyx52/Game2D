using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace Game.Graphics {
    public class Texture : IDisposable {
        private readonly int textureID;
        public readonly int width;
        public readonly int height;
        public Texture(int width, int height) : this(width, height, TextureWrapMode.ClampToEdge, TextureMinFilter.Nearest, TextureMagFilter.Nearest) {}
        public Texture(int width, int height, TextureWrapMode wrapMode, TextureMinFilter minFilter, TextureMagFilter magFilter) {
            this.width = width;
            this.height = height;
            this.textureID = GL.GenTexture();
            
            GL.BindTexture(TextureTarget.Texture2D, this.textureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)wrapMode);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public static Texture WhiteTexture {
            // NOTE: Use this with caution, it creates new texture instance everytime!
            get {
                Texture texture = new Texture(1, 1, TextureWrapMode.Repeat, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
                texture.SetData(new byte[4] {0xFF, 0xFF, 0xFF, 0xFF}, 1, 1);
                GC.KeepAlive(texture);
                return texture;       
            }
        }
        public int TextureID {
            get {
                return this.textureID;
            }
        }
        public int Width {
            get {
                return this.width;
            }
        }
        public int Height {
            get {
                return this.height;
            }
        }

        public void BindToUnit(int slot) {
            GL.BindTextureUnit(slot, this.textureID);
        }
        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, this.textureID);  
        }
        public void Unbind() {
            GL.BindTexture(TextureTarget.Texture2D, 0);    
        }
        public void GenerateMipmap() {
            this.Bind();
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            this.Unbind();
        }
        public void SetData(IntPtr data, int width, int height) {
            GL.TextureStorage2D(this.textureID, 1, SizedInternalFormat.Rgba8, width, height);
            GL.TextureSubImage2D(this.textureID, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            this.GenerateMipmap();  
        }
        public void SetData(in byte[] data, int width, int height) {
            GL.TextureStorage2D(this.textureID, 1, SizedInternalFormat.Rgba8, width, height);
            GL.TextureSubImage2D(this.textureID, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            this.GenerateMipmap();  
        }
        public void Dispose() {
            GameHandler.Logger.Warn("Disposing texture! Is this intentional?");
            GL.DeleteTexture(this.textureID);
        }
        public unsafe static Texture LoadTexture(string filePath) {
            try {
                // Load img and flip it
                Image<Rgba32> img = (Image<Rgba32>) Image.Load(filePath);
                img.Mutate(x => x.Flip(FlipMode.Vertical));
                
                // Create texture
                Texture texture = new Texture(img.Width, img.Height);

                // Get pointer to image data
                fixed (void* data = &MemoryMarshal.GetReference(img.GetPixelRowSpan(0)))
                {
                    // Set data using IntPtr
                    texture.SetData((IntPtr)data, img.Width, img.Height);
                }

                //Deleting the img and collect garbage
                img.Dispose();
                GC.Collect();

                GLHelper.CheckGLError($"Texture.LoadTexture<{filePath}>");
                return texture;
            } catch(IOException e) {
                GameHandler.Logger.Error($"Error while opening image! {e}");
            }
            return new Texture(1, 1);
        }
    }
}