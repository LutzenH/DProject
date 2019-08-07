using DProject.Entity;
using DProject.Entity.Interface;
using DProject.List;
using DProject.Manager.Entity;
using DProject.Manager.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI
{
    public class WorldEditorUI : AbstractUI, ILoadContent
    {
        private Texture2D _spritesheet;
        private SpriteFont _spriteFont;
        
        public WorldEditorUI(EditorEntityManager editorEntityManager, EditorUIManager editorUIManager) : base(editorEntityManager, editorUIManager) { }

        public void LoadContent(ContentManager content)
        {
            _spritesheet = content.Load<Texture2D>("textures/ui/editor");
            _spriteFont = content.Load<SpriteFont>("fonts/default");
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
            
            switch (EntityManager.GetWorldEditorEntity().GetCurrentTool())
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

            if(EntityManager.GetWorldEditorEntity().GetCurrentTool() != WorldEditorEntity.Tools.ObjectPlacer && EntityManager.GetWorldEditorEntity().GetCurrentTool() != WorldEditorEntity.Tools.Select)
                spriteBatch.Draw(
                    _spritesheet,
                    new Rectangle(2,Game1.ScreenResolutionY/2,32,32),
                    new Rectangle(EntityManager.GetWorldEditorEntity().GetBrushSize()*16, 16, 16, 16),
                    Color.White
                    );
            
            spriteBatch.Draw(
                _spritesheet,
                destinationRectangle,
                new Rectangle(80,0,16,16),
                Color.White
            );
            
            if(EntityManager.GetWorldEditorEntity().GetCurrentTool() == WorldEditorEntity.Tools.Flatten)
                spriteBatch.DrawString(_spriteFont, "FlattenHeight: " + EntityManager.GetWorldEditorEntity().GetFlattenHeight(), new Vector2(2, 19 * 2), Color.White);

            if (EntityManager.GetWorldEditorEntity().GetCurrentTool() == WorldEditorEntity.Tools.ObjectPlacer)
            {
                spriteBatch.DrawString(_spriteFont, "SelectedProp: " + Props.PropList[EntityManager.GetWorldEditorEntity().GetSelectedObject()].GetAssetPath() , new Vector2(2, 19 * 2), Color.White);
                spriteBatch.DrawString(_spriteFont, "Rotation: " + EntityManager.GetWorldEditorEntity().GetSelectedRotation().ToString() , new Vector2(2, 19 * 3), Color.White);
            }
        }
    }
}