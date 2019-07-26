using System;
using Microsoft.Xna.Framework;

namespace DProject.UI.Element.Ports
{
    public class TimeControlUIElement : AbstractUIElement
    {
        public enum TimeControlOption { None, Pause, Play, Speedup, End }

        private readonly Sprite _timerBackdrop;
        private readonly Sprite _timerBackdropDisabled;

        private readonly Sprite _timerPause;
        private readonly Sprite _timerPauseClicked;
        private readonly Sprite _timerPauseDisabled;

        private readonly Sprite _timerPlay;
        private readonly Sprite _timerPlayClicked;
        private readonly Sprite _timerPlayDisabled;

        private readonly Sprite _timerSpeedup;
        private readonly Sprite _timerSpeedupClicked;
        private readonly Sprite _timerSpeedupDisabled;

        private readonly Sprite _timerEnd;
        private readonly Sprite _timerEndClicked;
        private readonly Sprite _timerEndDisabled;

        private bool _enabled;
        private TimeControlOption _selectedOption;
        
        public Rectangle[] ButtonRectangles { get; }
        
        public TimeControlUIElement(Point position) : base(position)
        {
            _timerBackdrop = new Sprite(position, "ui_elements", "timer_backdrop");
            _timerBackdropDisabled = new Sprite(position, "ui_elements", "timer_backdrop_disabled");
            
            _timerPause = new Sprite(new Point(position.X - 48, position.Y), "ui_elements", "timer_pause");
            _timerPauseClicked = new Sprite(new Point(position.X - 48, position.Y), "ui_elements", "timer_pause_clicked");
            _timerPauseDisabled = new Sprite(new Point(position.X - 48, position.Y), "ui_elements", "timer_pause_disabled");

            _timerPlay = new Sprite(new Point(position.X - 16, position.Y), "ui_elements", "timer_play");
            _timerPlayClicked = new Sprite(new Point(position.X - 16, position.Y), "ui_elements", "timer_play_clicked");
            _timerPlayDisabled = new Sprite(new Point(position.X - 16, position.Y), "ui_elements", "timer_play_disabled");
            
            _timerSpeedup = new Sprite(new Point(position.X + 16, position.Y), "ui_elements", "timer_speedup");
            _timerSpeedupClicked = new Sprite(new Point(position.X + 16, position.Y), "ui_elements", "timer_speedup_clicked");
            _timerSpeedupDisabled = new Sprite(new Point(position.X + 16, position.Y), "ui_elements", "timer_speedup_disabled");
            
            _timerEnd = new Sprite(new Point(position.X + 48, position.Y), "ui_elements", "timer_end");
            _timerEndClicked = new Sprite(new Point(position.X + 48, position.Y), "ui_elements", "timer_end_clicked");
            _timerEndDisabled = new Sprite(new Point(position.X + 48, position.Y), "ui_elements", "timer_end_disabled");
            
            AddSprite(_timerBackdrop);
            AddSprite(_timerBackdropDisabled);
            AddSprite(_timerPause);
            AddSprite(_timerPauseClicked);
            AddSprite(_timerPauseDisabled);
            AddSprite(_timerPlay);
            AddSprite(_timerPlayClicked);
            AddSprite(_timerPlayDisabled);
            AddSprite(_timerSpeedup);
            AddSprite(_timerSpeedupClicked);
            AddSprite(_timerSpeedupDisabled);
            AddSprite(_timerEnd);
            AddSprite(_timerEndClicked);
            AddSprite(_timerEndDisabled);
            
            Enabled = true;
            SetClickedItem(TimeControlOption.None);
            
            ButtonRectangles = new[]
            {
                _timerPause.Rectangle,
                _timerPlay.Rectangle,
                _timerSpeedup.Rectangle,
                _timerEnd.Rectangle
            };
        }

        public bool Enabled
        {
            get => _enabled;
            set {
                if (value)
                {
                    SetSpriteVisibility(true, false, true, false, false, true, false,
                                    false, true, false, false, true, false, false);

                    _enabled = true;
                    SelectedButton = _selectedOption;
                }
                else
                {
                    SetSpriteVisibility(false, true, false, false, true, false, false,
                                    true, false, false, true, false, false, true);

                    _enabled = false;
                }
            }
        }
        
        public TimeControlOption SelectedButton {
            get => _selectedOption;
            set
            {
                _selectedOption = value;

                if (Enabled)
                    SetClickedItem(value);
            }
        }
        
        private void SetClickedItem(TimeControlOption option)
        {
            switch (option)
            {
                case TimeControlOption.None:
                    SetSpriteVisibility(timerPause:true, timerPauseClicked:false, timerPlay:true, timerPlayClicked:false,
                        timerSpeedup:true, timerSpeedupClicked:false, timerEnd:true, timerEndClicked:false);
                    break;
                case TimeControlOption.Pause:
                    SetSpriteVisibility(timerPause:false, timerPauseClicked:true, timerPlay:true, timerPlayClicked:false,
                        timerSpeedup:true, timerSpeedupClicked:false, timerEnd:true, timerEndClicked:false);
                    break;
                case TimeControlOption.Play:
                    SetSpriteVisibility(timerPause:true, timerPauseClicked:false, timerPlay:false, timerPlayClicked:true,
                        timerSpeedup:true, timerSpeedupClicked:false, timerEnd:true, timerEndClicked:false);
                    break;
                case TimeControlOption.Speedup:
                    SetSpriteVisibility(timerPause:true, timerPauseClicked:false, timerPlay:true, timerPlayClicked:false,
                        timerSpeedup:false, timerSpeedupClicked:true, timerEnd:true, timerEndClicked:false);
                    break;
                case TimeControlOption.End:
                    SetSpriteVisibility(timerPause:true, timerPauseClicked:false, timerPlay:true, timerPlayClicked:false,
                        timerSpeedup:true, timerSpeedupClicked:false, timerEnd:false, timerEndClicked:true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, "The given option does not exist");
            }
        }
        
        private void SetSpriteVisibility(
            bool? timerBackdrop = null,
            bool? timerBackdropDisabled = null,
            bool? timerPause = null,
            bool? timerPauseClicked = null,
            bool? timerPauseDisabled = null,
            bool? timerPlay = null,
            bool? timerPlayClicked = null,
            bool? timerPlayDisabled = null,
            bool? timerSpeedup = null,
            bool? timerSpeedupClicked = null,
            bool? timerSpeedupDisabled = null,
            bool? timerEnd = null,
            bool? timerEndClicked = null,
            bool? timerEndDisabled = null)
        {
            if (timerBackdrop != null) _timerBackdrop.Visible = (bool) timerBackdrop;
            if (timerBackdropDisabled != null) _timerBackdropDisabled.Visible = (bool) timerBackdropDisabled;
            if (timerPause != null) _timerPause.Visible = (bool) timerPause;
            if (timerPauseClicked != null) _timerPauseClicked.Visible = (bool) timerPauseClicked;
            if (timerPauseDisabled != null) _timerPauseDisabled.Visible = (bool) timerPauseDisabled;
            if (timerPlay != null) _timerPlay.Visible = (bool) timerPlay;
            if (timerPlayClicked != null) _timerPlayClicked.Visible = (bool) timerPlayClicked;
            if (timerPlayDisabled != null) _timerPlayDisabled.Visible = (bool) timerPlayDisabled;
            if (timerSpeedup != null) _timerSpeedup.Visible = (bool) timerSpeedup;
            if (timerSpeedupClicked != null) _timerSpeedupClicked.Visible = (bool) timerSpeedupClicked;
            if (timerSpeedupDisabled != null) _timerSpeedupDisabled.Visible = (bool) timerSpeedupDisabled;
            if (timerEnd != null) _timerEnd.Visible = (bool) timerEnd;
            if (timerEndClicked != null) _timerEndClicked.Visible = (bool) timerEndClicked;
            if (timerEndDisabled != null) _timerEndDisabled.Visible = (bool) timerEndDisabled;
        }
    }
} 
