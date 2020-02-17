using System;
using System.Collections.Generic;
using DProject.Game.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Type.Rendering
{
    public class ViewportRenderer
    {
        private const int MaxMapRadius = 4096;
        
        private GraphicsDevice _graphicsDevice;

        private BasicEffect _basicEffect;
        private RenderTarget2D _viewport;

        private readonly int _width;
        private readonly int _height;

        private float _zoom;
        private int _usableGridSize;
        private int _gridSize;

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (value < 0.000625f)
                    _zoom = 0.000625f;
                else if (value > 1.2)
                    _zoom = 1.2f;
                else
                    _zoom = value;
                
                UpdateProjection();
            }
}

        public int Width => _width;
        public int Height => _height;

        public int GridSize
        {
            get => _gridSize;
            set
            {
                if (value < 1)
                    _gridSize = 1;
                else if (value > MaxMapRadius)
                    _gridSize = MaxMapRadius;
                else
                    _gridSize = value;
            }
        }

        private Matrix _viewMatrix;
        private Matrix _scaleMatrix;
        private Matrix _projectionMatrix;

        private Vector2 _offset;
        public Vector2 Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                UpdateProjection();
            }
        }

        private ViewportDirection _direction;
        public ViewportDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                UpdateMatrices();
            }
        }

        public ViewportRenderer(int width, int height)
        {
            _width = width;
            _height = height;

            _zoom = 1;
            _gridSize = 64;
            Offset = new Vector2(0, 0);
            Direction = ViewportDirection.Front;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _basicEffect = new BasicEffect(graphicsDevice)
            {
                Alpha = 1.0f,
                LightingEnabled = false,
                VertexColorEnabled = true,
                TextureEnabled = false,
                Projection = _projectionMatrix
            };

            _viewport = new RenderTarget2D(graphicsDevice, _width, _height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.SetRenderTarget(_viewport);
            _graphicsDevice.Clear(Color.Black);

            var grid = GetGrid(_gridSize, _zoom);
            
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = Matrix.Identity;
            _basicEffect.Projection = _projectionMatrix;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, grid, 0, grid.Length/2);
            }
        }

        public void Dispose()
        {
            _viewport?.Dispose();
        }

        public RenderTarget2D GetViewport()
        {
            return _viewport;
        }

        private VertexPositionColor[] GetGrid(int gridSize, float zoom)
        {
            var topLeftPosition = _offset - new Vector2(zoom * MaxMapRadius);
            var bottomRightPosition = _offset + new Vector2(zoom * MaxMapRadius);
            
            var relativeSize = gridSize / zoom;
            _usableGridSize = gridSize;

            while (relativeSize <= 128)
            {
                _usableGridSize *= 2;
                relativeSize *= 2;
            }

            var list = new List<VertexPositionColor>();
            
            for (var x = -MaxMapRadius; x <= MaxMapRadius; x += _usableGridSize)
            {
                if (x < topLeftPosition.X || x > bottomRightPosition.X)
                    continue;

                var color = CalculateLineColor(x, _usableGridSize);

                list.Add(new VertexPositionColor(new Vector3(x, -MaxMapRadius, 0), color));
                list.Add(new VertexPositionColor(new Vector3(x, MaxMapRadius, 0), color));
            }
            
            for (var y = -MaxMapRadius; y <= MaxMapRadius; y += _usableGridSize)
            {
                if (y < topLeftPosition.Y || y > bottomRightPosition.Y)
                    continue;
                
                var color = CalculateLineColor(y, _usableGridSize);
                
                list.Add(new VertexPositionColor(new Vector3(-MaxMapRadius, y, 0), color));
                list.Add(new VertexPositionColor(new Vector3(MaxMapRadius,y, 0), color));
            }
            
            return list.ToArray();
        }

        private void UpdateMatrices()
        {
            var directionMatrix = Matrix.Identity;

            switch (_direction)
            {
                case ViewportDirection.Top:
                    directionMatrix = Matrix.CreateFromYawPitchRoll(0, -MathHelper.Pi/2, 0);
                    _scaleMatrix = Matrix.CreateScale(1, 1f, 0f);
                    break;
                case ViewportDirection.Left:
                    directionMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.Pi/2, 0, MathHelper.Pi);
                    _scaleMatrix = Matrix.CreateScale(1, 1, 0f);
                    break;
                case ViewportDirection.Front:
                    directionMatrix = Matrix.CreateFromYawPitchRoll(0, MathHelper.Pi, 0);
                    _scaleMatrix = Matrix.CreateScale(1, 1, 0f);
                    break;
            }

            _viewMatrix = directionMatrix * _scaleMatrix;
        }

        private static Color CalculateLineColor(int linePosition, int gridSize)
        {
            if (linePosition == 0)
                return new Color(50, 100, 100);
            
            if (linePosition % (gridSize*8) == 0)
                return new Color(100, 100, 100);

            return new Color(50, 50, 50);
        }

        private void UpdateProjection()
        {
            var projectionValue = MaxMapRadius * _zoom;
                
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(
                -projectionValue + _offset.X,
                projectionValue + _offset.X,
                projectionValue + _offset.Y,
                -projectionValue + _offset.Y,
                0, 1);
        }

        public Vector3 GetMousePosition(Vector2 relativeMousePositionWithinViewport)
        {
            var mousePosition = relativeMousePositionWithinViewport - new Vector2(0.5f, 0.5f);
            var relativeWorldPosition = mousePosition * _zoom * (MaxMapRadius * 2) + _offset;
            
            switch (_direction)
            {
                case ViewportDirection.Top:
                    return new Vector3(relativeWorldPosition.X, 0, relativeWorldPosition.Y);
                case ViewportDirection.Front:
                    return new Vector3(relativeWorldPosition.X, -relativeWorldPosition.Y, 0);
                case ViewportDirection.Left:
                    return new Vector3(0, -relativeWorldPosition.Y,  relativeWorldPosition.X);
            }

            return Vector3.Zero;
        }

        public void DrawMesh(DPModel model, TransformComponent transform)
        {
            _graphicsDevice.SetVertexBuffer(model.VertexBuffer);
            _graphicsDevice.Indices = model.IndexBuffer;

            _basicEffect.World = transform.WorldMatrix;
            _basicEffect.View = _viewMatrix;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.VertexBuffer.VertexCount, 0, model.PrimitiveCount);
            }
        }

        public int GetUsableGridSize()
        {
            return _usableGridSize;
        }
    }
    
    public enum ViewportDirection
    {
        Top,
        Front,
        Left
    }
}
