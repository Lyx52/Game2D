using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System;
using System.Runtime.InteropServices;
using Game.Utils;

namespace Game.Graphics {
    public class Texture : IDisposable {
        private readonly int TextureID;
        public readonly int Width;
        public readonly int Height;
        public Texture(int width, int height) : this(width, height, TextureWrapMode.ClampToEdge, TextureMinFilter.Nearest, TextureMagFilter.Nearest) {}
        public Texture(int width, int height, TextureWrapMode wrapMode, TextureMinFilter minFilter, TextureMagFilter magFilter) {
            this.Width = width;
            this.Height = height;
            this.TextureID = GL.GenTexture();
            
            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);
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
                return texture;       
            }
        }
        public override bool Equals(object obj)
        {
            bool value = this.TextureID == ((Texture)obj).TextureID;
            // Two textures must not have the same textureID!
            return value;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void BindToUnit(int slot) {
            GL.BindTextureUnit(slot, this.TextureID);
        }
        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);  
        }
        public void SetData(IntPtr data, int width, int height, SizedInternalFormat internalFormat=SizedInternalFormat.Rgba8, PixelFormat pixelFormat=PixelFormat.Rgba) {
            this.Bind();
            GL.TextureStorage2D(this.TextureID, 1, internalFormat, width, height);
            GL.TextureSubImage2D(this.TextureID, 0, 0, 0, width, height, pixelFormat, PixelType.UnsignedByte, data);
        }
        public void SetData(in byte[] data, int width, int height, SizedInternalFormat internalFormat=SizedInternalFormat.Rgba8, PixelFormat pixelFormat=PixelFormat.Rgba) {
            this.Bind();
            GL.TextureStorage2D(this.TextureID, 1, internalFormat, width, height);
            GL.TextureSubImage2D(this.TextureID, 0, 0, 0, width, height, pixelFormat, PixelType.UnsignedByte, data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void Dispose() {
            Logger.Warn("Disposing texture! Is this intentional?");
            GL.DeleteTexture(this.TextureID);
        }
        public unsafe static Texture Load(string filePath) {
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

                GLDebug.CheckGLError($"Texture.LoadTexture<{filePath}>");
                return texture;
            } catch(IOException e) {
                Logger.Error($"Error while opening image! {e}");
            }
            return new Texture(1, 1);
        }
    }
}