using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.UI
{
    public class WorldEditorUI : AbstractUI, IUpdateable
    {
        private Texture2D _spritesheet;
        private SpriteFont _spriteFont;

        private enum Tools { Flatten, Raise, Paint, ObjectPlacer }

        private Tools _tools;
        
        public WorldEditorUI() { }

        public override void LoadContent(ContentManager content)
        {
            _spritesheet = content.Load<Texture2D>("textures/ui/editor");
            _spriteFont = content.Load<SpriteFont>("default");
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.D1) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D1))
            {
                _tools = Tools.Flatten;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.D2) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D2))
            {
                _tools = Tools.Raise;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.D3) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D3))
            {
                _tools = Tools.Paint;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.D4) && Game1.PreviousKeyboardState.IsKeyDown(Keys.D4))
            {
                _tools = Tools.ObjectPlacer;
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(
                _spritesheet,
                new Rectangle(2,2,32,32),
                new Rectangle(0,0, 16,16),
                Color.White
            );
            
            spriteBatch.Draw(
                _spritesheet,
                new Rectangle(36,2,32,32),
                new Rectangle(16,0, 16,16),
                Color.White
            );
            
            spriteBatch.Draw(
                _spritesheet,
                new Rectangle(70,2,32,32),
                new Rectangle(32,0, 16,16),
                Color.White
            );
            
            spriteBatch.Draw(
                _spritesheet,
                new Rectangle(104,2,32,32),
                new Rectangle(48,0, 16,16),
                Color.White
            );

            Rectangle destinationRectangle;
            
            switch (_tools)
            {
                case Tools.Flatten:
                    destinationRectangle = new Rectangle(2, 2, 32, 32);
                    break;
                case Tools.Raise:
                    destinationRectangle = new Rectangle(36, 2, 32, 32);
                    break;
                case Tools.Paint:
                    destinationRectangle = new Rectangle(70, 2, 32, 32);
                    break;
                case Tools.ObjectPlacer:
                    destinationRectangle = new Rectangle(104, 2, 32, 32);
                    break;
                default:
                    destinationRectangle = new Rectangle(0, 0, 0, 0);
                    break;
            }
            
            spriteBatch.DrawString(_spriteFont, "WorldEditorUI", new Vector2(2, 34), Color.White);
            
            spriteBatch.Draw(
                _spritesheet,
                destinationRectangle,
                new Rectangle(64,0,16,16),
                Color.White
            );
        }
    }
}