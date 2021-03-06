using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Game.Utils;
using System;

namespace Game.Graphics {
    public class ShaderProgram : IDisposable {
        public readonly int programID;

        private Dictionary<string, int> attribLocations;
        private Dictionary<string, int> uniformLocations;
   
        public ShaderProgram(string vertFile, string fragFile) {
            this.programID = GL.CreateProgram();

            // Put all locations in dictionary to eliminate excess gl calls
            this.attribLocations = new Dictionary<string, int>();
            this.uniformLocations = new Dictionary<string, int>();

            string vertexSource = IOUtils.ReadTextFile(vertFile);
            this.AttachShader(vertexSource, ShaderType.VertexShader);

            string fragSource = IOUtils.ReadTextFile(fragFile);
            this.AttachShader(fragSource, ShaderType.FragmentShader);

            GL.LinkProgram(this.programID);
            GLHelper.CheckGLObjectError(this.programID, GLObject.PROGRAM);
        }
        
        public void AttachShader(string source, ShaderType type) {
            int shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, source);
            GL.CompileShader(shaderID);
            GLHelper.CheckGLObjectError(shaderID, GLObject.SHADER);

            GL.AttachShader(this.programID, shaderID);
            GL.DeleteShader(shaderID);
        }
        public int GetAttribLocation(string varName) {
            if (this.attribLocations.TryGetValue(varName, out int location)) {
                return location;
            } else {
                int new_location = GL.GetAttribLocation(this.programID, varName);
                this.attribLocations.Add(varName, new_location);
                return new_location;
            }
        }
        public int GetUniformLocation(string varName) {
            if (this.uniformLocations.TryGetValue(varName, out int location)) {
                return location;
            } else {
                int new_location = GL.GetUniformLocation(this.programID, varName);
                
                if (new_location < 0) {
                    GameHandler.Logger.Warn($"Shader uniform \"{varName}\" location could not be found!");
                } else {
                    this.uniformLocations.Add(varName, new_location);
                }
                return new_location;
            }
        }
        public void Bind() {
            GL.UseProgram(this.programID);
        }
        public void Set4f(Vector4 value, string varName) {
            GL.ProgramUniform4(this.programID, this.GetUniformLocation(varName), ref value);
        }
        public void Set3f(Vector3 value, string varName) {
            GL.ProgramUniform3(this.programID, this.GetUniformLocation(varName), ref value);
        }
        public void Set2f(Vector2 value, string varName) {
            GL.ProgramUniform2(this.programID, this.GetUniformLocation(varName), ref value);
        }
        public void Set1f(ref float value, string varName) {
            GL.ProgramUniform1(this.programID, this.GetUniformLocation(varName), value);
        }
        public void SetMat4(Matrix4 matrix, string varName) {
            GL.ProgramUniformMatrix4(this.programID, this.GetUniformLocation(varName), false, ref matrix);
        }
        public void SetSampler2D(int textureUnit, string varName) {
            GL.Uniform1(this.GetUniformLocation(varName), textureUnit);
        }
        public void Dispose() {
            GL.DeleteProgram(this.programID);
        }
    }
}