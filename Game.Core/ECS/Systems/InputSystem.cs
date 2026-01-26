using System.Numerics;
using Veldrid;

namespace Game.Core.ECS.Systems
{
    public class InputSystem : EcsSystem
    {
        public Vector2 MousePosition { get; private set; }
        public InputSnapshot FrameSnapshot { get; internal set; }
        public IWindow Window { get; set; }

        private HashSet<Key> _currentlyPressedKeys = [];
        private HashSet<Key> _newKeysThisFrame = [];
        private HashSet<MouseButton> _currentlyPressedMouseButtons = [];
        private HashSet<MouseButton> _newMouseButtonsThisFrame = [];

        public bool GetKey(Key key) => _currentlyPressedKeys.Contains(key);

        public bool GetKeyDown(Key key) => _newKeysThisFrame.Contains(key);

        public bool GetMouseButton(MouseButton button) => _currentlyPressedMouseButtons.Contains(button);

        public bool GetMouseButtonDown(MouseButton button) => _newMouseButtonsThisFrame.Contains(button);

        protected override void UpdateCore(double deltaTime)
        {
            Window ??= SystemRegistry.Get<IWindow>();
            FrameSnapshot = Window.Base.PumpEvents();

            _newKeysThisFrame.Clear();
            _newMouseButtonsThisFrame.Clear();

            MousePosition = FrameSnapshot.MousePosition;
            for (int i = 0; i < FrameSnapshot.KeyEvents.Count; i++)
            {
                KeyEvent ke = FrameSnapshot.KeyEvents[i];
                if (ke.Down)
                    KeyDown(ke.Key);
                else
                    KeyUp(ke.Key);
            }
            for (int i = 0; i < FrameSnapshot.MouseEvents.Count; i++)
            {
                MouseEvent me = FrameSnapshot.MouseEvents[i];
                if (me.Down)
                    MouseDown(me.MouseButton);
                else
                    MouseUp(me.MouseButton);
            }
        }

        private void MouseUp(MouseButton mouseButton)
        {
            _currentlyPressedMouseButtons.Remove(mouseButton);
            _newMouseButtonsThisFrame.Remove(mouseButton);
        }

        private void MouseDown(MouseButton mouseButton)
        {
            if (_currentlyPressedMouseButtons.Add(mouseButton))
                _newMouseButtonsThisFrame.Add(mouseButton);
        }

        private void KeyUp(Key key)
        {
            _currentlyPressedKeys.Remove(key);
            _newKeysThisFrame.Remove(key);
        }

        private void KeyDown(Key key)
        {
            if (_currentlyPressedKeys.Add(key))
                _newKeysThisFrame.Add(key);
        }
    }
}
