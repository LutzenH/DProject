using DProject.UI.Element.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DProject.UI.Element.Ports
{
    public class MapButtonUIElement : AbstractUIElement, IUpdateableUIElement
    {
        private readonly Sprite _mapSmall;
        private readonly Sprite _mapLarge;
        private readonly Sprite _mapDisabled;

        private bool _large;
        private bool _enabled;

        public Rectangle Rectangle { get; private set; }

        public MapButtonUIElement(Point position) : base(position)
        {
            _mapSmall = new Sprite(position, "ui_elements", "map_small");
            _mapLarge = new Sprite(position, "ui_elements", "map_large");
            _mapDisabled = new Sprite(position, "ui_elements", "map_disabled");
            
            AddSprite(_mapSmall);
            AddSprite(_mapLarge);
            AddSprite(_mapDisabled);

            Enabled = true;
            Large = false;
        }
        
        public void Update(Vector2 mousePosition, ref Rectangle? currentRectangleBeingDragged, ref (object, string)? pressedButton)
        {
            Large = Rectangle.Contains(mousePosition);
            if (Rectangle.Contains(mousePosition))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Released && Game1.PreviousMouseState.LeftButton == ButtonState.Pressed)
                    pressedButton = (this, "map_button");
            }
        }
        
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value)
                {
                    _enabled = true;
                    Large = _large;
                }
                else
                {
                    SetSpriteVisibility(false, false, true);
                    Rectangle = _mapSmall.Rectangle;
                    _enabled = false;
                }
            }
        }

        public bool Large
        {
            get => _large;
            set
            {
                if (_enabled)
                {
                    if (value)
                    {
                        SetSpriteVisibility(false, true, false);
                        Rectangle = _mapLarge.Rectangle;
                    }
                    else
                    {
                        SetSpriteVisibility(true, false, false);
                        Rectangle = _mapSmall.Rectangle;
                    }
                }

                _large = value;
            }
        }

        private void SetSpriteVisibility(bool? mapSmall = null, bool? mapLarge = null, bool? mapDisabled = null)
        {
            if (mapSmall != null) _mapSmall.Visible = (bool) mapSmall;
            if (mapLarge != null) _mapLarge.Visible = (bool) mapLarge;
            if (mapDisabled != null) _mapDisabled.Visible = (bool) mapDisabled;
        }
    }
} 
