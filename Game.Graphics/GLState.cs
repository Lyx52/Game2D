using OpenTK.Graphics.OpenGL4;

namespace Game.Graphics {
    public struct CapabilityState {
        public EnableCap Capability { get; }
        private bool CurrentState { get; set; }
        public CapabilityState(EnableCap capability) {
            this.Capability = capability;
            this.CurrentState = false;
        }
        public void setState(bool state) {
            if (this.CurrentState != state) {
                this.CurrentState = state;
                if (state) {
                    GL.Enable(this.Capability);
                } else {
                    GL.Disable(this.Capability);
                }
            }
            GLHelper.CheckGLError($"Set state {state} to {this.Capability}");
        }
    }

    public class AlphaState {
        private CapabilityState alphaFunction;
        public BlendingFactor sourceFactor;
        public BlendingFactor destinationFactor;
        public AlphaState() {
            this.alphaFunction = new CapabilityState(EnableCap.Blend);
            this.sourceFactor = BlendingFactor.SrcAlpha;
            this.destinationFactor = BlendingFactor.OneMinusSrcAlpha;
            
            GL.BlendFunc(this.sourceFactor, this.destinationFactor);
        }
        public void Enable() {
            this.alphaFunction.setState(true);
        }
        public void Disable() {
            this.alphaFunction.setState(false);
        }
        public void SetSourceFactor(BlendingFactor sfactor) {
            this.sourceFactor = sfactor;
            GL.BlendFunc(sfactor, this.destinationFactor);
        }
        public void SetDestinationFactor(BlendingFactor dfactor) {
            this.destinationFactor = dfactor;
            GL.BlendFunc(this.sourceFactor, dfactor);
        }
    }
    public class GLState {

        private AlphaState alphaState = new AlphaState();
        private QuadMode quadMode = QuadMode.CENTER;

        public QuadMode QuadRenderMode {
            get {
                return this.quadMode;
            }
            set {
                this.quadMode = value;
            }
        }
        public void EnableAlpha() {
            this.alphaState.Enable();
        }
        public void DisableAlpha() {
            this.alphaState.Disable();
        }
        public void SetClearColor(float red, float green, float blue, float alpha) {
            GL.ClearColor(red, green, blue, alpha);
        }
    }
}