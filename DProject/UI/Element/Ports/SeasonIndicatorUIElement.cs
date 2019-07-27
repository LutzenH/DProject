using System;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Ports
{
    public class SeasonIndicatorUIElement : AbstractUIElement
    {
        private readonly Sprite _seasonAutumnal;
        private readonly Sprite _seasonEstival;
        private readonly Sprite _seasonHibernal;
        private readonly Sprite _seasonPrevernal;
        private readonly Sprite _seasonSerotinal;
        private readonly Sprite _seasonVernal;
        
        private readonly Sprite _frame;

        public SeasonIndicatorUIElement(Point position) : base(position)
        {
            _seasonAutumnal = new Sprite(position, "ui_elements", "season_autumnal");
            _seasonEstival = new Sprite(position, "ui_elements", "season_estival");
            _seasonHibernal = new Sprite(position, "ui_elements", "season_hibernal");
            _seasonPrevernal = new Sprite(position, "ui_elements", "season_prevernal");
            _seasonSerotinal = new Sprite(position, "ui_elements", "season_serotinal");
            _seasonVernal = new Sprite(position, "ui_elements", "season_vernal");
            
            _frame = new Sprite(position, "ui_elements", "frame_simple");

            AddSprite(_seasonAutumnal);
            AddSprite(_seasonEstival);
            AddSprite(_seasonHibernal);
            AddSprite(_seasonPrevernal);
            AddSprite(_seasonSerotinal);
            AddSprite(_seasonVernal);
            
            AddSprite(_frame);
            
            SetSeason(Season.Vernal);
        }

        public void SetSeason(Season season)
        {
            switch (season)
            {
                case Season.Vernal:
                    SetSpriteVisibility(false, false, false, false, false, true);
                    break;
                case Season.Estival:
                    SetSpriteVisibility(false, true, false, false, false, false);
                    break;
                case Season.Serotinal:
                    SetSpriteVisibility(false, false, false, false, true, false);
                    break;
                case Season.Autumnal:
                    SetSpriteVisibility(true, false, false, false, false, false);
                    break;
                case Season.Hibernal:
                    SetSpriteVisibility(false, false, true, false, false, false);
                    break;
                case Season.Prevernal:
                    SetSpriteVisibility(false, false, false, true, false, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(season), season, "The given season does not exist");
            }
        }

        private void SetSpriteVisibility(bool? autumnal = null, bool? estival = null, bool? hibernal = null, bool? prevernal = null, bool? serotinal = null, bool? vernal = null)
        {
            if (autumnal != null) _seasonAutumnal.Visible = (bool) autumnal;
            if (estival != null) _seasonEstival.Visible = (bool) estival;
            if (hibernal != null) _seasonHibernal.Visible = (bool) hibernal;
            if (prevernal != null) _seasonPrevernal.Visible = (bool) prevernal;
            if (serotinal != null) _seasonSerotinal.Visible = (bool) serotinal;
            if (vernal != null) _seasonVernal.Visible = (bool) vernal;
        }
    }
} 
