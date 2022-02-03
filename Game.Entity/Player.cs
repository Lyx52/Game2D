using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;
using Game.World;
using System.Collections.Generic;

namespace Game.Entity {
    public class Player : DrawableEntity {
        private string PlayerName = "NoName";
        public Player(float x, float y) : base() {
            this.AttachComponent(new KinematicBody(x, y));
            this.AttachComponent(new EntityController(this));
            // this.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            this.Sprite = new AnimatedSprite(GameHandler.Renderer.GetTexture("pod_texture"), 8, 7, frameTime:0.25D);
            this.Animation.AddAnimation("pod_drive", 0, 7, frameTime:0.05D);
            this.Animation.PlayAnimation("pod_drive");
            this.Physics.Size = new Vector2(64, 64);
            this.Layer = RenderLayer.LAYER_2;
        }
        public override void Draw(Renderer renderer) {
            renderer.DispatchQuad(this.GetRenderQuad(this.Physics));
            renderer.GUI.DrawText("Player", (GameHandler.WindowSize / 2) + new Vector2(-32F, 20F), 0.5F, new Vector3(0.75F, 0.0F, 5.0F));      
        }
        public EntityController Controller {
            get { return (EntityController)this.GetComponent("EntityController"); }
        }
        public KinematicBody Physics {
            get { return (KinematicBody)this.GetComponent("KinematicBody"); }
        }
        public AnimatedSprite Animation {
            get { return (AnimatedSprite)this.Sprite;}
        }
        public override void Update(double dt) {
            this.Animation.Update(dt);
            // Rotate sprite towards mouse
            this.Physics.Rotation = MathUtils.LookAt(this.Physics.Position, this.Physics.Position + this.Controller.GlobalMousePosition);
            
            // Acceleration vector is equal to UP/DOWN key multiplied by acceleration
            Vector2 acceleration = new Vector2(0, this.Controller.GetDirectional().Y * (float)(dt *this.Physics.Acceleration));
            
            // We rotate acceleration vector to sprite angle and add it to velocity
            this.Physics.Velocity += MathUtils.Rotate(acceleration, this.Physics.Rotation * MathUtils.Deg2Rad);
            
            // We add velocity to position
            this.Physics.Position += this.Physics.Velocity;

            // We add drag to velocity
            this.Physics.Velocity *= this.Physics.Drag;
        }
        public override string ToString() {
            return "Player";
        }
        public override string GetParrent() {
            return base.ToString();
        }
        public CompoundTag GetPlayerTag() {
            return new CompoundTag("Player", new List<Tag>() {
                new StringTag("PlayerName", this.PlayerName),
                new DoubleTag("Acceleration", this.Physics.Acceleration),
                new FloatTag("Rotation", this.Physics.Rotation),
                new Vector2Tag("Position", this.Physics.Position)
            });
        }
        public void LoadPlayerData(CompoundTag playerTag) {
            Logger.Assert(playerTag.Name == "Player", "Invalid compound tag provided!");
            
            Logger.Assert(playerTag.Contains("Acceleration"), "Player tag doesnt contain Acceleration key!");
            this.Physics.Acceleration = playerTag.GetDoubleTag("Acceleration").Value;

            Logger.Assert(playerTag.Contains("Rotation"), "Player tag doesnt contain Rotation key!");
            this.Physics.Rotation = playerTag.GetFloatTag("Rotation").Value;

            Logger.Assert(playerTag.Contains("Position"), "Player tag doesnt contain Position key!");
            this.Physics.Position = playerTag.GetVector2Tag("Position").Value;

            Logger.Assert(playerTag.Contains("PlayerName"), "Player tag doesnt contain PlayerName key!");
            this.PlayerName = playerTag.GetStringTag("PlayerName").Value;
        }
        public override bool InRange(Entity target, float range)
        {
            return Vector2.Distance(this.Physics.Position, ((KinematicBody)target.GetComponent("KinematicBody")).Position) <= range;
        }
    }
}