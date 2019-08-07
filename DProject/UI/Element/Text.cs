using System;
using DProject.List;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.UI.Element
{
    public class Text
    {
        [Flags]
        public enum AlignmentType { Center=0, Left=1, Right=2, Top=4, Bottom = 8 }
        
        public bool Visible { get; set; }

        public bool DropShadow { get; set; }

        public string String { get; set; }

        public Color Color { get; set; }

        public AlignmentType Alignment { get; set; }

        public Rectangle Bounds { get; set; }

        public Point Offset { get; set; }

        private readonly string _collectionName;
        private readonly string _fontName;
        
        public Text(Rectangle bounds, string collectionName, string fontName, string text)
        {
            Bounds = bounds;
            Visible = true;
            String = text;
            Alignment = AlignmentType.Left;
            Color = Color.White;
            Offset = Point.Zero;

            _collectionName = collectionName;
            _fontName = fontName;
        }

        public Text(Rectangle bounds, string fontName, string text) : this(bounds, "default_fonts", fontName, text) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(Visible)
                DrawString(spriteBatch, Fonts.FontList[_collectionName][_fontName].SpriteFont, String, Bounds, Alignment, Color, Offset, DropShadow);
        }
        
        private static void DrawString(SpriteBatch spriteBatch, SpriteFont font, string text, Rectangle bounds, AlignmentType align, Color color, Point offset, bool dropShadow)
        {
            var size = font.MeasureString( text );
            var pos = bounds.Center + offset;
            var origin = size * 0.5f;

            if (align.HasFlag( AlignmentType.Left))
                origin.X += bounds.Width/2 - size.X/2;

            if (align.HasFlag( AlignmentType.Right))
                origin.X -= bounds.Width/2 - size.X/2;

            if (align.HasFlag( AlignmentType.Top))
                origin.Y += bounds.Height/2 - size.Y/2;

            if (align.HasFlag( AlignmentType.Bottom))
                origin.Y -= bounds.Height/2 - size.Y/2;

            if(dropShadow)
                spriteBatch.DrawString(font, text, new Vector2(pos.X + 1, pos.Y + 1), Color.Black, 0, origin, 1, SpriteEffects.None, 0);
            
            spriteBatch.DrawString(font, text, pos.ToVector2(), color, 0, origin, 1, SpriteEffects.None, 0);
        }
    }
} 
