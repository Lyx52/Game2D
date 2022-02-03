using System;
using System.Collections.Generic;
using Game.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game.Entity {
    public enum ControllerAction {
        NONE,
        MOVE_UP,
        MOVE_DOWN,
        MOVE_LEFT,
        MOVE_RIGHT,
        PICK_UP_ITEM
    }
    public class EntityController : EntityComponent{
        private Entity parrentEntity;
        private Dictionary<ControllerAction, int> actions;
        public EntityController(Entity parrent) {
            this.parrentEntity = parrent;
            this.actions = new Dictionary<ControllerAction, int>();
            this.InitDefaultKeybinds();
        }
        public int GetActionKeyCode(ControllerAction action) {
            if (this.actions.TryGetValue(action, out int keyCode)) {
                return keyCode;
            } else return 0;
        } 
        public Vector2 GetDirectional() {
            return new Vector2(
                GetActionKeyI(ControllerAction.MOVE_RIGHT) - GetActionKeyI(ControllerAction.MOVE_LEFT),
                GetActionKeyI(ControllerAction.MOVE_UP) - GetActionKeyI(ControllerAction.MOVE_DOWN)
            );
        }
        public int GetActionKeyI(ControllerAction action) {
            return KeyboardHandler.GetKeyI(GetActionKeyCode(action));    
        }
        public bool GetActionKey(ControllerAction action) {
            return KeyboardHandler.GetKey(GetActionKeyCode(action));    
        }
        public bool GetMouseKey(Input.MouseButton button) {
            return MouseHandler.GetButton(button);
        }
        public void InitDefaultKeybinds() {
            foreach (ControllerAction action in Enum.GetValues(typeof(ControllerAction))) {
                switch(action) {
                    case ControllerAction.MOVE_UP: {
                        this.actions.Add(action, (int)Keys.W);
                    } break;
                    case ControllerAction.MOVE_DOWN: {
                        this.actions.Add(action, (int)Keys.S);
                    } break;
                    case ControllerAction.MOVE_LEFT: {
                        this.actions.Add(action, (int)Keys.A);
                    } break;
                    case ControllerAction.MOVE_RIGHT: {
                        this.actions.Add(action, (int)Keys.D);
                    } break;
                    case ControllerAction.PICK_UP_ITEM: {
                        this.actions.Add(action, (int)Keys.F);
                    } break;
                }
            } 
        }
        public Vector2 MousePosition {
            get { return MouseHandler.GetPosition(); }
        }
        public Vector2 GlobalMousePosition {
            get { return (this.MousePosition - GameHandler.WindowSize / 2); }
        }
        public override string ToString() {
            return "ControllerComponent";
        }
    }
}