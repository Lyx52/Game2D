using System;
using FreeTypeSharp;
using static FreeTypeSharp.Native.FT;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace Game.Graphics {
    struct Character {
        public int TexID;  // ID handle of the glyph texture
        public Vector2i Size;       // Size of glyph
        public Vector2i Bearing;    // Offset from baseline to left/top of glyph
        public uint Advance;    // Offset to advance to next glyph
        public Character(int texID, Vector2i size, Vector2i bearing, uint adv) {
            this.TexID = texID;
            this.Size = size;
            this.Bearing = bearing;
            this.Advance = adv;
        }
    }

    public class TextHandler : IDisposable {
        private static FreeTypeSharp.FreeTypeLibrary FreeTypeLib;
        public static FreeTypeFaceFacade CurrentFace;
        private static Dictionary<char, Character> CharacterMap;
        private ShaderProgram TextShader;
        private VertexArray<uint, Vector4> TextVertexArray;
        private static uint CurrentFontSize = 24;
        public unsafe TextHandler(int MAX_CHARS=256) {
            CharacterMap = new Dictionary<char, Character>();
            
            // Init FreeType
            CurrentFace = default(FreeTypeFaceFacade);
            FreeTypeLib = new FreeTypeLibrary();
            FT_Library_Version(FreeTypeLib.Native, out var major, out var minor, out var patch);
            GameHandler.Logger.Debug($"FreeType version: {major}.{minor}.{patch}");

            // Init GL objects
            this.TextShader = new ShaderProgram("./res/shaders/TextShader.vert", "./res/shaders/TextShader.frag");
            
            // We set the textu projection matrix once and leave it
            this.TextShader.Bind();
            Matrix4.CreateOrthographicOffCenter(0.0F, GameHandler.WindowSize.X, 0.0F, GameHandler.WindowSize.Y, 0.0F, 1.0F, out Matrix4 projection);
            this.TextShader.SetMat4(projection, "textProjection");

            this.TextVertexArray = new VertexArray<uint, Vector4>(new VertexLayout(
                new List<VertexElement>() {
                    new VertexElement("vertex", ElementType.Vector4f, 0) 
                }), 6, MAX_CHARS * 4
            );
        }
        public static void ReadFont(string filePath, uint size=48) {
            if (FT_New_Face(FreeTypeLib.Native, filePath, 0, out IntPtr face) != FreeTypeSharp.Native.FT_Error.FT_Err_Ok) {
                GameHandler.Logger.Critical($"Error! Could not read font file {filePath}!");
            }
            // Set current font face
            CurrentFace = new FreeTypeFaceFacade(FreeTypeLib, face);
            SetFontSize(24);
        }
        public static void SetFontSize(uint size) {
            CurrentFontSize = size;
            FT_Set_Pixel_Sizes(CurrentFace.Face, 0, size);
            LoadChars();
        }
        public static void DeleteCharacterMap() {
            // Delete current char map
            foreach (Character chr in CharacterMap.Values) {
                GL.DeleteTexture(chr.TexID);
            }
            CharacterMap.Clear();
        }
        private static void LoadChars() {
            if (CharacterMap.Count > 0)
                DeleteCharacterMap();
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);   
            for (uint i = 32; i < 128; i++) {
                // Load char bitmap
                FreeTypeSharp.Native.FT_Error code = FT_Load_Char(CurrentFace.Face, i, FT_LOAD_RENDER);
                if (code != FreeTypeSharp.Native.FT_Error.FT_Err_Ok) {
                    GameHandler.Logger.Critical($"Freetype {code} error could not load glyph for {(char)i} char using font {CurrentFace.MarshalFamilyName()}!");
                }
                
                // Bind texture
                int texID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texID);


                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.R8,
                    (int)CurrentFace.GlyphBitmap.width,
                    (int)CurrentFace.GlyphBitmap.rows,
                    0,
                    PixelFormat.Red,
                    PixelType.UnsignedByte,
                    CurrentFace.GlyphBitmap.buffer
                );

                // set texture options
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
                GLHelper.CheckGLError($"TextHandler::Char {(char)i} glyph creation");
                if (!CharacterMap.TryAdd((char)i, 
                    new Character(texID, new Vector2i((int)CurrentFace.GlyphBitmap.width, (int)CurrentFace.GlyphBitmap.rows), new Vector2i((int)CurrentFace.GlyphBitmapLeft, (int)CurrentFace.GlyphBitmapTop), (uint)CurrentFace.GlyphMetricHorizontalAdvance))
                ){
                    GameHandler.Logger.Error($"Cannot add already existing char {(char)i} to glyph dictionary!");
                }
            }
        }
        public void RenderText(string text, Vector2 position, float scale, Vector3 color, int fontSize=48) {
            // We cannot draw text if we dont have any glyph sizes
            if (CharacterMap.Count <= 0)
                return;
            SetFontSize((uint)fontSize);
            this.TextShader.Bind();
            this.TextShader.Set3f(color, "textColor");

            GL.ActiveTexture(TextureUnit.Texture0);
            this.TextVertexArray.Bind();

            foreach (char c in text) {
                if (CharacterMap.TryGetValue(c, out Character ch)) {
                    float xpos = position.X + ch.Bearing.X * scale;
                    float ypos = position.Y - (ch.Size.Y - ch.Bearing.Y) * scale;

                    float w = ch.Size.X * scale;
                    float h = ch.Size.Y * scale;
                    // update VBO for each character

                    Vector4[] vertices = new Vector4[6] {
                        new Vector4( xpos,     ypos + h,   0.0f, 0.0f ),
                        new Vector4( xpos,     ypos,       0.0f, 1.0f ),
                        new Vector4( xpos + w, ypos,       1.0f, 1.0f ),

                        new Vector4( xpos,     ypos + h,   0.0f, 0.0f ),
                        new Vector4( xpos + w, ypos,       1.0f, 1.0f ),
                        new Vector4( xpos + w, ypos + h,   1.0f, 0.0f ),
                    };
                    GL.BindTexture(TextureTarget.Texture2D, ch.TexID);
                    this.TextVertexArray.VertexBuffer.SetSubData(vertices);
                    Renderer.DrawPrimative(PrimitiveType.Triangles, 6);
                    position.X += ch.Advance * scale;
                } else {
                    GameHandler.Logger.Error($"Character doesn't contain {c} char!");
                }
            }
        }
        public void Dispose() {
            // Dispose of freetype and font face
            FT_Done_Face(CurrentFace.Face);
            FT_Done_FreeType(FreeTypeLib.Native);
            DeleteCharacterMap();
        }
    }
}