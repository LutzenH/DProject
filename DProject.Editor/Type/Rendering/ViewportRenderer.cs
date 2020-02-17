using System.Collections.Generic;
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

                var projectionValue = MaxMapRadius * _zoom;
                
                _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                    -projectionValue,
                    projectionValue,
                    projectionValue,
                    -projectionValue,
                    0, 1);
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

        public ViewportRenderer(int width, int height)
        {
            _width = width;
            _height = height;

            _zoom = 1;
            _gridSize = 64;
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
                Projection = Matrix.CreateOrthographicOffCenter(
                    -MaxMapRadius,
                    MaxMapRadius,
                    MaxMapRadius,
                    -MaxMapRadius,
                    0, 1)
            };

            _viewport = new RenderTarget2D(graphicsDevice, _width, _height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.SetRenderTarget(_viewport);
            _graphicsDevice.Clear(Color.Black);

            var grid = GetGrid(_gridSize, _zoom);
            
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

        //TODO: Only draw lines that are within the given zoom level.
        private VertexPositionColor[] GetGrid(int gridSize, float zoom)
        {
            var relativeSize = gridSize / zoom;
            var usableGridSize = gridSize;

            while (relativeSize <= 128)
            {
                usableGridSize *= 2;
                relativeSize *= 2;
            }

            var list = new List<VertexPositionColor>();
            
            for (var x = -MaxMapRadius; x <= MaxMapRadius; x += usableGridSize)
            {
                var color = new Color(50, 50, 50);

                if (x % (usableGridSize*8) == 0)
                    color = new Color(100, 100, 100);
                
                if (x == 0)
                    color = new Color(50, 100, 100);

                list.Add(new VertexPositionColor(new Vector3(x, -MaxMapRadius, 0), color));
                list.Add(new VertexPositionColor(new Vector3(x, MaxMapRadius, 0), color));
                
                list.Add(new VertexPositionColor(new Vector3(-MaxMapRadius, x, 0), color));
                list.Add(new VertexPositionColor(new Vector3(MaxMapRadius,x, 0), color));
            }
            
            return list.ToArray();
        }

        public Vector2 GetMousePosition(Vector2 relativeMousePositionWithinViewport)
        {
            var mousePosition = relativeMousePositionWithinViewport - new Vector2(0.5f, 0.5f);
            return mousePosition * _zoom * (MaxMapRadius * 2);
        }
    }
}
