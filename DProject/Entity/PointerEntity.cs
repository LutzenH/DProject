using DProject.Entity.Interface;
using DProject.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IDrawable = DProject.Entity.Interface.IDrawable;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity
{
    public class PointerEntity : AbstractEntity, IInitialize,IUpdateable, IDrawable
    {
        private readonly AxisEntity _axisEntity;
        private readonly EntityManager _entityManager;
        private GraphicsDevice _graphicsDevice;

        public PointerEntity(EntityManager entityManager) : base(Vector3.Zero, Quaternion.Identity, new Vector3(1,1,1))
        {
            _axisEntity = new AxisEntity(Vector3.Zero);
            _entityManager = entityManager;
        }

        public override void LoadContent(ContentManager content) { }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _axisEntity.Initialize(graphicsDevice);
            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            
            Vector3? tilePosition = Game1.GetTilePosition(
                new float[64,64],
                mouseLocation,
                _entityManager.GetActiveCamera().GetViewMatrix(),
                _entityManager.GetActiveCamera().GetProjectMatrix(),
                _graphicsDevice.Viewport
                );

            if (tilePosition != null)
                _axisEntity.SetPosition((Vector3) tilePosition);
            else
                _axisEntity.SetPosition(Vector3.Zero);

            foreach (var entity in _entityManager.GetEntities())
            {
                if (entity is PropEntity propEntity)
                {
                    if (Game1.Intersects(
                        new Vector2(mouseLocation.X, mouseLocation.Y),
                        propEntity.GetModel(),
                        propEntity.GetWorldMatrix(),
                        _entityManager.GetActiveCamera().GetViewMatrix(),
                        _entityManager.GetActiveCamera().GetProjectMatrix(),
                        _graphicsDevice.Viewport))
                    {
                        _axisEntity.SetPosition(propEntity.GetPosition());
                    }
                }
            }
        }

        public void Draw(CameraEntity activeCamera)
        {
            _axisEntity.Draw(activeCamera);
        }
    }
}