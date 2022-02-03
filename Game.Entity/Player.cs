using OpenTK.Mathematics;
using Game.Graphics;
using Game.Utils;
using Game.World;
using System.Collections.Generic;
using System;

namespace Game.Entity {
    public class Player : DrawableEntity {
        private string PlayerName = "NoName";
        private ControllerAction LastAction = ControllerAction.NONE;
        public Player(float x, float y) : base() {
            this.AttachComponent(new KinematicBody(x, y));
            this.AttachComponent(new EntityController(this));
            // this.Sprite.SpriteTexture = GameHandler.Renderer.GetTexture("grass");
            this.Sprite = new AnimatedSprite(GameHandler.Renderer.GetTexture("player_texture"), 47, 1, frameTime:0.1D);

            // Add idle animations
            this.Animation.AddAnimation("down_idle", 0, 4);
            this.Animation.AddAnimation("side_idle", 5, 9);
            this.Animation.AddAnimation("up_idle", 10, 14);
            

            // Add walk animations
            this.Animation.AddAnimation("down_walk", 15, 20);
            this.Animation.AddAnimation("side_walk", 21, 26);
            this.Animation.AddAnimation("up_walk", 27, 32);

            // Add attack animations
            this.Animation.AddAnimation("down_attack", 33, 35);
            this.Animation.AddAnimation("side_attack", 36, 38);
            this.Animation.AddAnimation("up_attack", 39, 41);

            // Add pickup animation
            this.Animation.AddAnimation("pick_up", 42, 46, frameTime: 0.25F, reset: false);

            this.Animation.PlayAnimation("up_idle");
            this.Physics.Size = new Vector2(64, 64);
            this.Layer = RenderLayer.LAYER_2;
        }
        public override void Draw(Renderer renderer) {
            renderer.DispatchQuad(this.GetRenderQuad(this.Physics));
            renderer.GUI.DrawText($"{this.Animation.CurrentAnimation}_frame_{this.Animation.CurrentFrameIndex}", (GameHandler.WindowSize / 2) + new Vector2(-32F, 20F), 0.5F, new Vector3(0.75F, 0.0F, 5.0F));      
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
            
            if (this.Controller.GetDirectional() == Vector2.Zero) {
                // We are idle
                switch(this.LastAction) {
                    case ControllerAction.MOVE_UP: {
                        this.Animation.PlayAnimation("up_idle");
                    } break;
                    case ControllerAction.MOVE_DOWN: {
                        this.Animation.PlayAnimation("down_idle");
                    } break;
                    case ControllerAction.MOVE_RIGHT: {
                        this.Animation.PlayAnimation("side_idle");
                        this.Animation.IsFlipped = true;
                    } break;
                    case ControllerAction.MOVE_LEFT: {
                        this.Animation.PlayAnimation("side_idle");
                        this.Animation.IsFlipped = false;
                    } break;
                }
            } else {
                if ((int)this.Controller.GetDirectional().X == 1) {
                    this.LastAction = ControllerAction.MOVE_RIGHT;
                    this.Animation.IsFlipped = true;
                    this.Animation.PlayAnimation("side_walk");
                } else if ((int)this.Controller.GetDirectional().X == -1) {
                    this.LastAction = ControllerAction.MOVE_LEFT;
                    this.Animation.IsFlipped = false;
                    this.Animation.PlayAnimation("side_walk");
                } else {
                    // We are moving
                    switch((int)this.Controller.GetDirectional().Y) {
                        case 1: {
                            this.LastAction = ControllerAction.MOVE_UP;
                            this.Animation.PlayAnimation("up_walk");
                        } break;
                        case -1: {
                            this.LastAction = ControllerAction.MOVE_DOWN;
                            this.Animation.PlayAnimation("down_walk");
                        } break;
                    }
                }
            }
            if (this.Controller.GetActionKey(ControllerAction.PICK_UP_ITEM)) {
                this.Animation.PlayAnimation("pick_up");
            }

            this.Physics.Velocity += this.Controller.GetDirectional() * (float)(this.Physics.Acceleration * dt);

            // We add velocity to position
            this.Physics.Position += this.Physics.Velocity;

            // We add drag to velocity
            this.Physics.Velocity *= this.Physics.Drag;
            this.Physics.Velocity = MathUtils.ClampVelocity(this.Physics.Velocity, 0.0005F, 5.0F);
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