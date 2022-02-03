using System.Collections.Generic;
using OpenTK.Mathematics;
using Game.Utils;
namespace Game.Graphics {
    public class AnimatedSprite : SpriteSheet {
        public uint CurrentFrameIndex = 0;
        public uint MaxFrameIndex = 0;
        private double AnimationTime = 0;
        public double FrameTime = 0;
        private double DefaultFrameTime;
        public string CurrentAnimation = "None";
        public Dictionary<string, (uint startIndex, uint endIndex, double frameTime, bool reset)> Animations;
        public AnimatedSprite(Texture texture, int cols, int rows, double frameTime=1D) : base(texture, cols, rows) {
            this.Type = SpriteType.ANIMATED_SPRITE;
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    this.AddNamedSubSprite($"frame_{MaxFrameIndex++}", j, i);
                }
            }
            MaxFrameIndex--;
            this.DefaultFrameTime = frameTime;
            this.FrameTime = frameTime;
            this.Animations = new Dictionary<string, (uint startIndex, uint endIndex, double frameTime, bool reset)>();
            this.Animations.Add(CurrentAnimation, (startIndex: CurrentFrameIndex, endIndex: MaxFrameIndex, frameTime: DefaultFrameTime, reset: true));
        }
        public void Update(double dt) {
            this.AnimationTime += dt;
            if (this.AnimationTime >= FrameTime) {
                this.AnimationTime = 0;
                this.StepPlayback();
            }

        }
        public void StepPlayback() {
            this.CurrentFrameIndex++;
            if (this.CurrentFrameIndex > this.Animations[this.CurrentAnimation].endIndex) {
                this.CurrentFrameIndex = this.Animations[this.CurrentAnimation].reset ? 0 : this.CurrentFrameIndex - 1;
            }
        }
        public void PlayAnimation(string animationName) {
            if (this.Animations.TryGetValue(animationName, out var animation)) {
                this.CurrentFrameIndex = animation.startIndex;
                this.FrameTime = animation.frameTime;
                this.AnimationTime = 0;
                this.CurrentAnimation = animationName;
                return;
            }
            Logger.Error($"Animation {animationName} doesn't exist!");
        }
        public void AddAnimation(string animationName, uint startIndex, uint endIndex) {
            this.AddAnimation(animationName, startIndex, endIndex, frameTime:this.DefaultFrameTime);
        }
        public void AddAnimation(string animationName, uint startIndex, uint endIndex, double frameTime, bool reset=true) {
            Logger.Assert(startIndex < MaxFrameIndex && endIndex <= MaxFrameIndex, $"Cannot add animation {animationName}, start/end index must be less than {MaxFrameIndex}!");
            if (this.Animations.TryAdd(animationName, (startIndex:startIndex, endIndex:endIndex, frameTime: frameTime, reset:reset)))
                return;

            Logger.Error($"Error while trying to add animation {animationName}, it already exists!");
        }
        public override Vector2[] GetTexCoords()
        {
            return this.GetNamedSubSprite($"frame_{CurrentFrameIndex}");
        }
    }
}