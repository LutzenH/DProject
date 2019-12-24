using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.Manager
{
    public enum Input
    {
        //Camera
        CameraMoveUp,
        CameraMoveDown,
        CameraMoveForward,
        CameraMoveBackwards,
        CameraMoveLeft,
        CameraMoveRight,
        CameraLookUp,
        CameraLookDown,
        CameraLookLeft,
        CameraLookRight,
        CameraIncreasedSpeed,
        CameraFreeRotation,
        
        //Debug
        DebugShowWireFrame,
        DebugHaltTerrain,
        DebugShowClipMaps,
        //Debug World
        DebugIncreaseViewDistance,
        DebugDecreaseViewDistance,
        PickupPhysicsBody
    }

    public sealed class InputManager
    {
        private static InputManager _instance;
        private InputManager() { }
        public static InputManager Instance
        {
            get => _instance ?? (_instance = new InputManager());
            set => _instance = value;
        }

        private HashSet<Input> _input = new HashSet<Input>();
        private HashSet<Input> _previousInput;

        private MouseState _previousMouseState;
        private MouseState _currentMouseState;

        //Camera
        public Vector2 CameraLookVector;

        public void Update()
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _previousInput = _input;
            _input = new HashSet<Input>();
            
            var pressedKeys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in pressedKeys)
            {
                switch (key)
                {
                    case Keys.W:
                        _input.Add(Input.CameraMoveForward);
                        break;
                    case Keys.A:
                        _input.Add(Input.CameraMoveLeft);
                        break;
                    case Keys.S:
                        _input.Add(Input.CameraMoveBackwards);
                        break;
                    case Keys.D:
                        _input.Add(Input.CameraMoveRight);
                        break;
                    case Keys.Q:
                        _input.Add(Input.CameraMoveUp);
                        break;
                    case Keys.E:
                        _input.Add(Input.CameraMoveDown);
                        break;
                    case Keys.LeftShift:
                        _input.Add(Input.CameraIncreasedSpeed);
                        break;
                    case Keys.Up:
                        _input.Add(Input.CameraLookUp);
                        break;
                    case Keys.Down:
                        _input.Add(Input.CameraLookDown);
                        break;
                    case Keys.Left:
                        _input.Add(Input.CameraLookLeft);
                        break;
                    case Keys.Right:
                        _input.Add(Input.CameraLookRight);
                        break;
                    case Keys.Tab:
                        _input.Add(Input.DebugShowWireFrame);
                        break;
                    case Keys.LeftControl:
                        _input.Add(Input.DebugHaltTerrain);
                        break;
                    case Keys.F3:
                        _input.Add(Input.DebugShowClipMaps);
                        break;
                    case Keys.OemPlus:
                        _input.Add(Input.DebugIncreaseViewDistance);
                        break;
                    case Keys.OemMinus:
                        _input.Add(Input.DebugDecreaseViewDistance);
                        break;
                }
            }

            if (_currentMouseState.MiddleButton == ButtonState.Pressed)
                _input.Add(Input.CameraFreeRotation);
            if (_currentMouseState.RightButton == ButtonState.Pressed)
                _input.Add(Input.PickupPhysicsBody);

            CameraLookVector = _previousMouseState.Position.ToVector2() - _currentMouseState.Position.ToVector2();
        }

        public bool IsInputDown(Input input)
        {
            return _input.Contains(input);
        }
        
        public bool IsInputUp(Input input)
        {
            return !_input.Contains(input);
        }

        public bool WasInputDown(Input input)
        {
            return _previousInput.Contains(input);
        }
        
        public bool WasInputUp(Input input)
        {
            return !_previousInput.Contains(input);
        }

        public bool IsInputPressed(Input input)
        {
            return IsInputUp(input) && WasInputDown(input);
        }
    }
}
