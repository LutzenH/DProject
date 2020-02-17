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
        PickupPhysicsBody,
        
#if EDITOR
        //Editor
        ViewportZoomIn,
        ViewportZoomOut,
        ViewportIncreaseGridSize,
        ViewportDecreaseGridSize,
        ViewportMoveRight,
        ViewportMoveLeft,
        ViewportMoveUp,
        ViewportMoveDown,
#endif
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

            //TODO: Replace this case-machine with something a little more efficient and scalable.
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
#if EDITOR
                    case Keys.Up:
                        _input.Add(Input.ViewportMoveUp);
                        break;
                    case Keys.Down:
                        _input.Add(Input.ViewportMoveDown);
                        break;
                    case Keys.Left:
                        _input.Add(Input.ViewportMoveLeft);
                        break;
                    case Keys.Right:
                        _input.Add(Input.ViewportMoveRight);
                        break;
#endif
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
#if EDITOR
                        _input.Add(Input.ViewportIncreaseGridSize);
#endif
                        break;
                    case Keys.OemMinus:
                        _input.Add(Input.DebugDecreaseViewDistance);
#if EDITOR
                        _input.Add(Input.ViewportDecreaseGridSize);
#endif

                        break;
                }
            }

            if (_currentMouseState.MiddleButton == ButtonState.Pressed)
                _input.Add(Input.CameraFreeRotation);
            if (_currentMouseState.RightButton == ButtonState.Pressed)
                _input.Add(Input.PickupPhysicsBody);
            
#if EDITOR
            if (_currentMouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
                _input.Add(Input.ViewportZoomOut);
            if (_currentMouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
                _input.Add(Input.ViewportZoomIn);
#endif
            
            CameraLookVector = new Vector2(_previousMouseState.X, _previousMouseState.Y)
                               - new Vector2(_currentMouseState.X, _currentMouseState.Y);
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
