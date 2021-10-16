using OpenTK.Graphics.OpenGL4;

namespace Game.Graphics {
    public enum GLObject {
        SHADER,
        PROGRAM,
        FRAMEBUFFER
    }
    public class GLHelper {
        public static readonly TextureUnit DefaultTextureUnit = TextureUnit.Texture0;
        public static void DisplayGLInfo() {
            string extensions = GL.GetString(StringName.Extensions);
            GameHandler.Logger.Info($"OpenGL Context: {GL.GetString(StringName.Version)}");
            GameHandler.Logger.Info($"OpenGL Renderer: {GL.GetString(StringName.Renderer)}");
            GameHandler.Logger.Info($"OpenGL Extensions: {(extensions.Length > 0 ? extensions : "None")}");
            GameHandler.Logger.Info($"GLSL Version: {GL.GetString(StringName.ShadingLanguageVersion)}");
        }
        public static string GetGLErrorString(ErrorCode code) {
            switch(code) {
                case ErrorCode.OutOfMemory: return "Out of Memory";
                case ErrorCode.InvalidValue: return "Invalid Value";
                case ErrorCode.InvalidOperation: return "Invalid Operation";
                case ErrorCode.InvalidFramebufferOperation: return "Invalid Framebuffer Operation";
                case ErrorCode.InvalidEnum: return "Invalid Enum";
                default: return "Unkown ErrorCode";
            }
        }

        public static void CheckGLError(string stage) {
            #if OPENGL_DEBUG
            ErrorCode errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError) {
                GameHandler.Logger.Error($"GL ERROR! @{stage}, {errorCode}: {GetGLErrorString(errorCode)}");
            }
            #endif
        }

        public static void CheckGLObjectError(int objectID, GLObject objectType) {
            #if OPENGL_DEBUG
            switch(objectType) {
                case GLObject.PROGRAM: {
                    GL.GetProgram(objectID, GetProgramParameterName.LinkStatus, out int linkStatus);
                    if (linkStatus == 0) {
                        GL.GetProgramInfoLog(objectID, out string info);
                        GameHandler.Logger.Error($"Error while linking ShaderProgram: {(info.Length > 0 ? info : "Not linked!")}");
                    }    
                } break;
                case GLObject.SHADER: {
                    GL.GetShader(objectID, ShaderParameter.CompileStatus, out int compileStatus);
                    if (compileStatus == 0) {
                        GL.GetShaderInfoLog(objectID, out string info);
                        GameHandler.Logger.Error($"Error while compiling ID({objectID}) - {GLHelper.GetShaderType(objectID)}: {(info.Length > 0 ? info : "Not compiled!")}");
                    }
                } break;
                case GLObject.FRAMEBUFFER: {
                    FramebufferErrorCode errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                    switch(errorCode) {
                        case FramebufferErrorCode.FramebufferComplete: break;
                        case FramebufferErrorCode.FramebufferIncompleteAttachment: {
                            GameHandler.Logger.Error($"Error not all framebuffer attachment points are framebuffer attachment complete!");
                        } break;
                        case FramebufferErrorCode.FramebufferIncompleteMissingAttachment: {
                            GameHandler.Logger.Error($"Error no images are attached to the framebuffer!");
                        } break;
                        case FramebufferErrorCode.FramebufferUnsupported: {
                            GameHandler.Logger.Error($"Error internal formats of the attached images violates an implementation-dependent set of restrictions!");
                        } break;
                        default: {
                            GameHandler.Logger.Error($"Unkown framebuffer error! {errorCode}");    
                        } break;
                    }
                } break;
            }
            #endif
        }
        public static string GetShaderType(int shaderID) {
            GL.GetShader(shaderID, ShaderParameter.ShaderType, out int shaderType);
            switch (shaderType) {
                case 35633: return "VertexShaderARB";
                case 35632: return "FragmentShaderARB";
                default: return $"Unkown Type({shaderType})";
            }    
        }
    }
}