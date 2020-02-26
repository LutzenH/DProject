using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager.World
{
    public class DefaultGameWorld : GameWorld
    {
        public DefaultGameWorld(Game1 game, ContentManager contentManager, GraphicsDevice graphicsDevice) : base(game, contentManager, graphicsDevice)
        {
            for (var i = 0; i < 400; i++)
            {
                EntityFactory.CreatePhysicsDynamicPrimitive(new Vector3(i%3/3f, i, i%4/3f), Vector3.One, Quaternion.Identity, PrimitiveType.Sphere);
            }

            CreateRedBrickBuilding();
            CreateWhiteMarbleBuilding();
            CreateCityBlock();

            //Entities
            EntityFactory.CreateFlyCamera(new Vector3(0, 0, 0));
            EntityFactory.CreateProp(new Vector3(0, 0, 0), "models/ports/shelves_empty");
            EntityFactory.CreateProp(new Vector3(-2, 0, 0), "models/ports/barrelstack");
            EntityFactory.CreateProp(new Vector3(-4, 0, 0), "models/ports/seat");
            EntityFactory.CreateProp(new Vector3(-6, 0, 0), "models/ports/stool");
            EntityFactory.CreateProp(new Vector3(-8, 0, 0), "models/ports/boat");

            EntityFactory.CreatePhysicsProp(new Vector3(0f, 10f, 0f), Vector3.One, "models/primitives/companion_cube");

            //Lights
            EntityFactory.CreateDirectionalLight(Vector3.Forward + Vector3.Down, Color.White);
            EntityFactory.CreateDirectionalLight(Vector3.Left, Color.White);
            EntityFactory.CreatePointLight(new Vector3(-0, 2, 0), Color.Red, 25f, 1f);
        }
        
        private void CreateRedBrickBuilding()
        {
            for (var i = 0; i < 5; i++)
            {
                EntityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_window");
                
                if(i == 2)
                    EntityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_ground_floor_door");
                else
                    EntityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_ground_floor_window");
                
                
                EntityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_rows");
                EntityFactory.CreateProp(new Vector3(-18, 0.3f, -7.8f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_parapet_side");
            }
            for (var i = 0; i < 6; i++)
                EntityFactory.CreateProp(new Vector3(-18, 0.3f, -9.75f + (i*3.9f)), "models/city/red_brick_building/red_brick_building_column");
        }

        private void CreateWhiteMarbleBuilding()
        {
            EntityFactory.CreateProp(new Vector3(-18, 0.3f, -18.5f), "models/city/white_marble_building/white_marble_building_ground_wall");
            EntityFactory.CreateProp(new Vector3(-18, 0.3f, -18.5f), "models/city/white_marble_building/white_marble_building_ground_pillars");

            EntityFactory.CreateProp(new Vector3(-18, 5.6f, -18.5f), "models/city/white_marble_building/white_marble_building_storey");
            EntityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f, -18.5f), "models/city/white_marble_building/white_marble_building_storey");
            EntityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f*2, -18.5f), "models/city/white_marble_building/white_marble_building_storey");
            EntityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f*3, -18.5f), "models/city/white_marble_building/white_marble_building_storey");

            EntityFactory.CreateProp(new Vector3(-18, 5.6f+3.2f*4, -18.5f), "models/city/white_marble_building/white_marble_building_roof");
        }

        private void CreateCityBlock()
        {
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-250, 0, -250), new Vector3(250, -1, 250), PrimitiveType.Cube);

            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-212f, 0.3f, 60f), new Vector3(212.0f, -3.4f, 36f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-212f, 0.0f, 36f), new Vector3(212.0f, -4f, 20f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-8f, 0.0f, 20f), new Vector3(8.0f, -4f, -140f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-212f, 0.3f, 20f), new Vector3(-8.0f, -3.4f, 10f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(8, 0.3f, 20f), new Vector3(212.0f, -3.4f, 10f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-18, 0.3f, 10f), new Vector3(-8.0f, -3.4f, -140f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(8, 0.3f, 10f), new Vector3(18.0f, -3.4f, -140f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-19, 19f, -10f), new Vector3(-86.0f, 0.3f, -28f), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-26, 19f, -28), new Vector3(-36.0f, 5f, -32), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-18, 21f, -32), new Vector3(-86.0f, 0.3f, -56), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(18, 18f, 10), new Vector3(94.0f, 0.3f, -54), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(-18,  0.3f, -56), new Vector3(-212.0f, -3.4f, -62), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(18,  0.3f, -54), new Vector3(212.0f, -3.4f, -62), PrimitiveType.Cube);
            EntityFactory.CreatePhysicsStaticPrimitive(new Vector3(18,  13f, -62), new Vector3(56.0f, 0.3f, -126), PrimitiveType.Cube);
        }
    }
} 
