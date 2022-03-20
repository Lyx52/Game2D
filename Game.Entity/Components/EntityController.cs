using System;
using System.Collections.Generic;
using Game.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game.Entity {
    public enum ControllerAction {
        MOVE_UP,
        MOVE_DOWN,
        MOVE_LEFT,
        MOVE_RIGHT
    }
    public class EntityController : EntityComponent{
        private Entity parrentEntity;
        private KeyboardHandler keyboardHandler;
        private MouseHandler mouseHandler;
        private Dictionary<ControllerAction, int> actions;

        public EntityController(Entity parrent) {
            this.parrentEntity = parrent;
            this.actions = new Dictionary<ControllerAction, int>();
            this.InitDefaultKeybinds();
        }
        public void AttachKeyboardHandler(KeyboardHandler handler) {
            this.keyboardHandler = handler;
        }
        public void AttachMouseHandler(MouseHandler handler) {
            this.mouseHandler = handler;
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
                        this.actions.Add(action, (int)Keys.W); // W
                    } break;
                    case ControllerAction.MOVE_DOWN: {
                        this.actions.Add(action, (int)Keys.S); // S
                    } break;
                    case ControllerAction.MOVE_LEFT: {
                        this.actions.Add(action, (int)Keys.A); // A
                    } break;
                    case ControllerAction.MOVE_RIGHT: {
                        this.actions.Add(action, (int)Keys.D); // D
                    } break;
                }
            } 
        }
        public Vector2 MousePosition {
            get { return this.mouseHandler.GetPosition(); }
        }
        public Vector2 GlobalMousePosition {
            get { return (this.MousePosition - GameHandler.WindowSize / 2); }
        }
        public override string ToString() {
            return "ControllerComponent";
        }
    }
}