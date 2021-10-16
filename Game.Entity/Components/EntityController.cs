using System;
using System.Collections.Generic;
using Game.Input;
using OpenTK.Mathematics;

namespace Game.Entity {
    public enum ControllerAction {
        MOVE_UP,
        MOVE_DOWN,
        MOVE_LEFT,
        MOVE_RIGHT
    }
    public class EntityController {
        private Entity parrentEntity;
        private KeyboardHandler keyboardHandler;
        private Dictionary<ControllerAction, int> actions;

        public EntityController(Entity parrent) {
            this.parrentEntity = parrent;
            this.actions = new Dictionary<ControllerAction, int>();
            this.InitDefaultKeybinds();
        }
        public void AttachKeyboardHandler(KeyboardHandler handler) {
            this.keyboardHandler = handler;
        }
        public int GetActionKeyCode(ControllerAction action) {
            if (this.actions.TryGetValue(action, out int keyCode)) {
                return keyCode;
            } else return 0;
        } 
        public Vector2 GetDirectional() {
            return new Vector2(
                this.keyboardHandler.GetKeyI(GetActionKeyCode(ControllerAction.MOVE_RIGHT)) - this.keyboardHandler.GetKeyI(GetActionKeyCode(ControllerAction.MOVE_LEFT)),
                this.keyboardHandler.GetKeyI(GetActionKeyCode(ControllerAction.MOVE_UP)) - this.keyboardHandler.GetKeyI(GetActionKeyCode(ControllerAction.MOVE_DOWN))
            );
        }
        public void InitDefaultKeybinds() {
            foreach (ControllerAction action in Enum.GetValues(typeof(ControllerAction))) {
                switch(action) {
                    case ControllerAction.MOVE_UP: {
                        this.actions.Add(action, 25); // W
                    } break;
                    case ControllerAction.MOVE_DOWN: {
                        this.actions.Add(action, 39); // S
                    } break;
                    case ControllerAction.MOVE_LEFT: {
                        this.actions.Add(action, 38); // A
                    } break;
                    case ControllerAction.MOVE_RIGHT: {
                        this.actions.Add(action, 40); // D
                    } break;
                }
            } 
        }
    }
}