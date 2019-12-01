using DProject.Game;
using DProject.Manager.System;
using DProject.Manager.System.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager.World
{
    public class GameWorld : WorldBuilder
    {
        private EntityFactory _entityFactory;

        public MonoGame.Extended.Entities.World World { get; }

        public GameWorld(ContentManager contentManager, ShaderManager shaderManager, GraphicsDevice graphicsDevice)
        {
            _entityFactory = new EntityFactory();

            //Camera
            AddSystem(new CameraSystem());

            //Models
            AddSystem(new ModelLoaderSystem(contentManager));
            AddSystem(new ModelRenderSystem(graphicsDevice, shaderManager));

            //Physics
            var physicsSystem = new PhysicsSystem();
            AddSystem(physicsSystem);
            AddSystem(new PhysicsSyncSystem(physicsSystem.GetSimulation()));
            AddSystem(new PhysicsMouseObjectDetectSystem(physicsSystem.GetSimulation()));
            
            //Primitives
            AddSystem(new PrimitiveRenderSystem(graphicsDevice, shaderManager));
            
            //Lighting
            AddSystem(new LightingRenderSystem(graphicsDevice, shaderManager));
            
            //Water
            AddSystem(new WaterRenderSystem(graphicsDevice, shaderManager));
            
#if EDITOR
            AddSystem(new DebugUIRenderSystem(graphicsDevice, shaderManager));            
#endif

            World = Build();
            _entityFactory.World = World;
            
            for (var i = 0; i < 400; i++)
            {
                _entityFactory.CreatePhysicsDynamicPrimitive(new Vector3(i%3/3f, i, i%4/3f), Vector3.One, Quaternion.Identity, PrimitiveType.Sphere);
            }

            CreateRedBrickBuilding();
            CreateWhiteMarbleBuilding();
            CreateCityBlock();

            //Entities
            _entityFactory.CreateFlyCamera(new Vector3(0, 0, 0));
            _entityFactory.CreateProp(new Vector3(0, 0, 0), 10);
            _entityFactory.CreateProp(new Vector3(-2, 0, 0), 9);
            _entityFactory.CreateProp(new Vector3(-4, 0, 0), 8);
            _entityFactory.CreateProp(new Vector3(-6, 0, 0), 7);
            _entityFactory.CreateProp(new Vector3(-8, 0, 0), 6);

            //Lights
            _entityFactory.CreateDirectionalLight(Vector3.Forward + Vector3.Down, Color.White);
            _entityFactory.CreateDirectionalLight(Vector3.Left, Color.White);
            _entityFactory.CreatePointLight(new Vector3(-0, 2, 0), Color.Red, 25f, 1f);
        }

        private void CreateRedBrickBuilding()
        {
            for (var i = 0; i < 5; i++)
            {
                _entityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_window");
                
                if(i == 2)
                    _entityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_ground_floor_door");
                else
                    _entityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_ground_floor_window");
                
                
                _entityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_rows");
                _entityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_parapet_side");
            }
            for (var i = 0; i < 6; i++)
                _entityFactory.CreateProp(new Vector3(-18, 0.3f, -9.75f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_column");
        }

        private void CreateWhiteMarbleBuilding()
        {
            _entityFactory.CreateProp(new Vector3(-18, 0.3f, -18.5f), "models/city/white_marble_building/white_marble_building_ground_wall");
            _entityFactory.CreateProp(new Vector3(-18, 0.3f, -18.5f), "models/city/white_marble_building/white_marble_building_ground_pillars");

            _entityFactory.CreateProp(new Vector3(-18, 5.6f, -18.5f), "models/city/white_marble_building/white_marble_building_storey");
            _entityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f, -18.5f), "models/city/white_marble_building/white_marble_building_storey");
            _entityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f*2, -18.5f), "models/city/white_marble_building/white_marble_building_storey");
            _entityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f*3, -18.5f), "models/city/white_marble_building/white_marble_building_storey");

            _entityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f*4, -18.5f), "models/city/white_marble_building/white_marble_building_roof");
        }

        private void CreateCityBlock()
        {
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-250, 0, -250), new Vector3(250, -1, 250), PrimitiveType.Cube);

            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-212f, 0.3f, 60f), new Vector3(212.0f, -3.4f, 36f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-212f, 0.0f, 36f), new Vector3(212.0f, -4f, 20f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-8f, 0.0f, 20f), new Vector3(8.0f, -4f, -140f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-212f, 0.3f, 20f), new Vector3(-8.0f, -3.4f, 10f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(8, 0.3f, 20f), new Vector3(212.0f, -3.4f, 10f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-18, 0.3f, 10f), new Vector3(-8.0f, -3.4f, -140f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(8, 0.3f, 10f), new Vector3(18.0f, -3.4f, -140f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-19, 19f, -10f), new Vector3(-86.0f, 0.3f, -28f), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-26, 19f, -28), new Vector3(-36.0f, 5f, -32), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-18, 21f, -32), new Vector3(-86.0f, 0.3f, -56), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(18, 18f, 10), new Vector3(94.0f, 0.3f, -54), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(-18,  0.3f, -56), new Vector3(-212.0f, -3.4f, -62), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(18,  0.3f, -54), new Vector3(212.0f, -3.4f, -62), PrimitiveType.Cube);
            _entityFactory.CreatePhysicsStaticPrimitive(new Vector3(18,  13f, -62), new Vector3(56.0f, 0.3f, -126), PrimitiveType.Cube);
        }
    }
}
