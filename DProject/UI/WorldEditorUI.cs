using DProject.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public class WorldEditorUI : AbstractUI
    {
        private Texture2D _spritesheet;
        private SpriteFont _spriteFont;
        
        private readonly WorldEditorEntity _worldEditorEntity;
        
        public WorldEditorUI(WorldEditorEntity worldEditorEntity)
        {
            _worldEditorEntity = worldEditorEntity;
        }

        public override void LoadContent(ContentManager content)
        {
            _spritesheet = content.Load<Texture2D>("textures/ui/editor");
            _spriteFont = content.Load<SpriteFont>("default");
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
            
            spriteBatch.Draw(
                _spritesheet,
                new Rectangle(136,2,32,32),
                new Rectangle(64,0, 16,16),
                Color.White
            );

            Rectangle destinationRectangle;
            
            switch (_worldEditorEntity.GetCurrentTool())
            {
                case WorldEditorEntity.Tools.Select:
                    destinationRectangle = new Rectangle(2, 2, 32, 32);
                    break;
                case WorldEditorEntity.Tools.Flatten:
                    destinationRectangle = new Rectangle(36, 2, 32, 32);
                    break;
                case WorldEditorEntity.Tools.Raise:
                    destinationRectangle = new Rectangle(70, 2, 32, 32);
                    break;
                case WorldEditorEntity.Tools.Paint:
                    destinationRectangle = new Rectangle(104, 2, 32, 32);
                    break;
                case WorldEditorEntity.Tools.ObjectPlacer:
                    destinationRectangle = new Rectangle(136, 2, 32, 32);
                    break;
                default:
                    destinationRectangle = new Rectangle(0, 0, 0, 0);
                    break;
            }

            if(_worldEditorEntity.GetCurrentTool() != WorldEditorEntity.Tools.ObjectPlacer && _worldEditorEntity.GetCurrentTool() != WorldEditorEntity.Tools.Select)
                spriteBatch.Draw(
                    _spritesheet,
                    new Rectangle(2,Game1.ScreenResolutionY/2,32,32),
                    new Rectangle(_worldEditorEntity.GetBrushSize()*16, 16, 16, 16),
                    Color.White
                    );
            
            spriteBatch.Draw(
                _spritesheet,
                destinationRectangle,
                new Rectangle(80,0,16,16),
                Color.White
            );
            
            if(_worldEditorEntity.GetCurrentTool() == WorldEditorEntity.Tools.Flatten)
                spriteBatch.DrawString(_spriteFont, "FlattenHeight: " + _worldEditorEntity.GetFlattenHeight(), new Vector2(2, 19 * 2), Color.White);
        }
    }
}