using DProject.Entity;
using DProject.List;
using DProject.Manager;
using DProject.UI.Element;
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
        private Texture2D _textureSpritesheet;
        
        private readonly WorldEditorEntity _worldEditorEntity;

        private SideBoxUIElement _sideBoxUIElement;

        private readonly Rectangle[] _sourceRectangles;
        private readonly Rectangle[] _destinationRectangles;
        
        public WorldEditorUI(WorldEditorEntity worldEditorEntity)
        {
            _worldEditorEntity = worldEditorEntity;
            
            _sourceRectangles = new Rectangle[Textures.TextureList.Count];
            _destinationRectangles = new Rectangle[Textures.TextureList.Count];
        }

        public override void LoadContent(ContentManager content)
        {
            _spritesheet = content.Load<Texture2D>("textures/ui/editor");
            _textureSpritesheet = content.Load<Texture2D>("textures/textureatlas");
            _spriteFont = content.Load<SpriteFont>("default");

            int i = 0;
            foreach (var texture in Textures.TextureList)
            {
                Vector4 texturePosition = texture.Value.GetTexturePosition();

                _sourceRectangles[i] = new Rectangle((int) texturePosition.X, (int) texturePosition.Y, 16, 16);
                _destinationRectangles[i] = new Rectangle((i * 32) % 96 + (4), i / 3 * 32 + (Game1.ScreenResolutionY / 2 + 36 + 36), 32, 32);

                i++;
            }
                
            _sideBoxUIElement = new SideBoxUIElement(new Point(0, Game1.ScreenResolutionY/2 + 36), new Point(96,192), _spritesheet);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            _sideBoxUIElement.Visible = false;
            
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

            if (_worldEditorEntity.GetCurrentTool() == WorldEditorEntity.Tools.Paint)
            {
                _sideBoxUIElement.Visible = true;
                
                for (int i = 0; i < _sourceRectangles.Length; i++)
                    spriteBatch.Draw(_textureSpritesheet, _destinationRectangles[i], _sourceRectangles[i], Color.White);
            }
            
            spriteBatch.Draw(
                _spritesheet,
                destinationRectangle,
                new Rectangle(80,0,16,16),
                Color.White
            );
            
            if(_worldEditorEntity.GetCurrentTool() == WorldEditorEntity.Tools.Flatten)
                spriteBatch.DrawString(_spriteFont, "FlattenHeight: " + _worldEditorEntity.GetFlattenHeight(), new Vector2(2, 19 * 2), Color.White);

            if (_worldEditorEntity.GetCurrentTool() == WorldEditorEntity.Tools.ObjectPlacer)
            {
                spriteBatch.DrawString(_spriteFont, "SelectedProp: " + Props.PropList[_worldEditorEntity.GetSelectedObject()].GetAssetName() , new Vector2(2, 19 * 2), Color.White);
                spriteBatch.DrawString(_spriteFont, "Rotation: " + _worldEditorEntity.GetSelectedRotation().ToString() , new Vector2(2, 19 * 3), Color.White);
            }

            _sideBoxUIElement.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed || Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                int i = 0;
                foreach (var texture in Textures.TextureList)
                {
                    if (_destinationRectangles[i].Contains(Mouse.GetState().Position))
                    {
                        UIManager.ClickedUI = true;
                        _worldEditorEntity.SetActiveTexture(texture.Key);
                    }

                    i++;
                }
            }
        }
    }
}