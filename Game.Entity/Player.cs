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
            this.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            this.Layer = RenderLayer.LAYER_2;
        }
        public override void Draw(Renderer renderer) {
            renderer.DispatchQuad(this.GetRenderQuad(this.KinematicBody));
            renderer.GUI.DrawText("Player", (GameHandler.WindowSize / 2) + new Vector2(-32F, 20F), 0.5F, new Vector3(0.75F, 0.0F, 5.0F));      
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
            //Vector2 acceleration = new Vector2(0, this.Controller.GetDirectional().Y * (float)(dt *this.KinematicBody.Acceleration));
            Vector2 acceleration = new Vector2(0, (float)(dt *this.KinematicBody.Acceleration));
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
            return new CompoundTag("Player", new List<Tag>() {
                new StringTag("PlayerName", this.PlayerName),
                new DoubleTag("Acceleration", this.KinematicBody.Acceleration),
                new FloatTag("Rotation", this.KinematicBody.Rotation),
                new Vector2Tag("Position", this.KinematicBody.Position)
            });
        }
        public void LoadPlayerData(CompoundTag playerTag) {
            Logger.Assert(playerTag.Name == "Player", "Invalid compound tag provided!");
            
            Logger.Assert(playerTag.Contains("Acceleration"), "Player tag doesnt contain Acceleration key!");
            this.KinematicBody.Acceleration = playerTag.GetDoubleTag("Acceleration").Value;

            Logger.Assert(playerTag.Contains("Rotation"), "Player tag doesnt contain Rotation key!");
            this.KinematicBody.Rotation = playerTag.GetFloatTag("Rotation").Value;

            Logger.Assert(playerTag.Contains("Position"), "Player tag doesnt contain Position key!");
            this.KinematicBody.Position = playerTag.GetVector2Tag("Position").Value;

            Logger.Assert(playerTag.Contains("PlayerName"), "Player tag doesnt contain PlayerName key!");
            this.PlayerName = playerTag.GetStringTag("PlayerName").Value;
        }
        public override bool InRange(Entity target, float range)
        {
            return Vector2.Distance(this.KinematicBody.Position, ((KinematicBody)target.GetComponent("KinematicBody")).Position) <= range;
        }
    }
}