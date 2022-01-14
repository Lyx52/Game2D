using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;
using Game.World;
using System.Collections.Generic;

namespace Game.Entity {
    public class Player : DrawableEntity {
        public Player(float x, float y) : base() {
            this.AttachComponent(new KinematicBody(x, y));
            this.AttachComponent(new EntityController(this));
            this.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            this.Layer = RenderLayer.LAYER_2;
        }
        public override void Draw(Renderer renderer) {
            renderer.DispatchQuad(this.GetRenderQuad(this.KinematicBody));         
        }
        public EntityController Controller {
            get { return (EntityController)this.GetComponent("EntityController"); }
        }
        public KinematicBody KinematicBody {
            get { return (KinematicBody)this.GetComponent("KinematicBody"); }
        }

        public override void Update(double dt) {
            // Rotate sprite towards mouse
            this.KinematicBody.Rotation = MathUtils.LookAt(this.KinematicBody.Position, this.KinematicBody.Position + this.Controller.GlobalMousePosition);
            
            // Acceleration vector is equal to UP/DOWN key multiplied by acceleration
            Vector2 acceleration = new Vector2(0, this.Controller.GetDirectional().Y * (float)(dt *this.KinematicBody.Acceleration));
            // We rotate acceleration vector to sprite angle and add it to velocity
            this.KinematicBody.Velocity += MathUtils.Rotate(acceleration, this.KinematicBody.Rotation * MathUtils.Deg2Rad);
            
            // We add velocity to position
            this.KinematicBody.Position += this.KinematicBody.Velocity;

            // We add drag to velocity
            this.KinematicBody.Velocity *= this.KinematicBody.Drag;
        }
        public override string ToString() {
            return "Player";
        }
        public override string GetParrent() {
            return base.ToString();
        }
        public CompoundTag GetPlayerTag() {
            return new CompoundTag("player", new List<Tag>() {
                new StringTag("PlayerName", "NoName"),
                new DoubleTag("Acceleration", this.KinematicBody.Acceleration),
                new FloatTag("Rotation", this.KinematicBody.Rotation),
                new CompoundTag("Position", new List<Tag>(){
                    new FloatTag("X", this.KinematicBody.Position.X),
                    new FloatTag("Y", this.KinematicBody.Position.Y)
                })
            });
        }
        public void LoadPlayerData(CompoundTag playerTag) {
            GameHandler.Logger.Assert(playerTag.Name == "player", "Invalid compound tag provided!");
            this.KinematicBody.Acceleration = playerTag.GetDoubleTag("Acceleration").Value;
            this.KinematicBody.Rotation = playerTag.GetFloatTag("Rotation").Value;
            
            CompoundTag position = playerTag.GetCompoundTag("Position");
            this.KinematicBody.Position = new Vector2(position.GetFloatTag("X").Value, position.GetFloatTag("Y").Value);
            GameHandler.Logger.Debug($"Position {this.KinematicBody.Position}");
        }
        public override bool InRange(Entity target, float range)
        {
            return Vector2.Distance(this.KinematicBody.Position, ((KinematicBody)target.GetComponent("KinematicBody")).Position) <= range;
        }
    }
}