using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.IO;

namespace Game.Graphics {
    public class Texture {
        private readonly int textureID;
        public readonly int width;
        public readonly int height;

        public Texture(int width, int height) {
            this.width = width;
            this.height = height;
            this.textureID = GL.GenTexture();
            
            GL.BindTexture(TextureTarget.Texture2D, this.textureID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public static Texture WhiteTexture {
            get {
                Texture texture = new Texture(1, 1);
                texture.SetData(new byte[4] {0xFF, 0xFF, 0xFF, 0xFF}, 1, 1);
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
        public void SetData(byte[] data, int width, int height) {
            GL.TextureStorage2D(this.textureID, 1, SizedInternalFormat.Rgba8, width, height);
            GL.TextureSubImage2D(this.textureID, 0, 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            this.GenerateMipmap();  
        }
        public void Delete() {
            GL.DeleteTexture(this.textureID);
        }
        public static Texture LoadTexture(string filePath) {
            try {
                Image<Rgba32> image = Image.Load<Rgba32>(filePath);
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                //Convert ImageSharp's format into a byte array, so we can use it with OpenGL.
                List<byte> pixels = new List<byte>(4 * image.Width * image.Height);

                for (int y = 0; y < image.Height; y++) {
                    var row = image.GetPixelRowSpan(y);

                    for (int x = 0; x < image.Width; x++) {
                        pixels.Add(row[x].R);
                        pixels.Add(row[x].G);
                        pixels.Add(row[x].B);
                        pixels.Add(row[x].A);
                    }
                }
                Texture texture = new Texture(image.Width, image.Height);
                texture.SetData(pixels.ToArray(), image.Width, image.Height);

                GLHelper.CheckGLError($"Texture.LoadTexture<{filePath}>");
                return texture;
            } catch(IOException e) {
                GameHandler.Logger.Error($"Error while opening image! {e}");
            }
            return new Texture(1, 1);
        }
    }
}