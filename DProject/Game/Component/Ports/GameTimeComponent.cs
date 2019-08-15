using System;
using DProject.Type.Enum;

namespace DProject.Game.Component.Ports
{
    public class GameTimeComponent
    {
        public uint CurrentDay { get; set; }
        
        public uint CurrentTime { get; set; }
        
        public Season CurrentSeason { get; set; }

        public double CurrentSecond { get; set; }

        private bool _paused;
        private float _gameSpeed;

        public GameTimeComponent()
        {
            CurrentDay = 0;
            CurrentTime = 0;
            Paused = false;
            GameSpeed = 1.0f;

            CurrentSecond = 0d;
            CurrentSeason = Season.Vernal;
        }
        
        public bool Paused {
            get => _paused;
            set
            {
                if (_paused != value)
                {
                    var args = new PauseStateChangedEventArgs()
                    {
                        PreviousPauseState = _paused
                    };
                    
                    OnPauseStateChanged(args);
                    
                    _paused = value;
                }
            }
        }
        
        public float GameSpeed {
            get => _gameSpeed;
            set
            {
                if (Math.Abs(_gameSpeed - value) > 0.05f)
                {
                    var args = new GameSpeedChangedEventArgs()
                    {
                        PreviousGameSpeed = _gameSpeed,
                        NewGameSpeed = value
                    };
                    
                    OnGameSpeedChanged(args);
                    
                    _gameSpeed = value;
                }
            }
        }
        
        #region Events

        public event EventHandler<SeasonChangedEventArgs> SeasonChanged;

        public virtual void OnSeasonChanged(SeasonChangedEventArgs e)
        {
            var handler = SeasonChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<DayChangedEventArgs> DayChanged;

        public virtual void OnDayChanged(DayChangedEventArgs e)
        {
            var handler = DayChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<TimeChangedEventArgs> TimeChanged;

        public virtual void OnTimeChanged(TimeChangedEventArgs e)
        {
            var handler = TimeChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<PauseStateChangedEventArgs> PauseStateChanged;
        protected virtual void OnPauseStateChanged(PauseStateChangedEventArgs e)
        {
            var handler = PauseStateChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<GameSpeedChangedEventArgs> GameSpeedChanged;
        protected virtual void OnGameSpeedChanged(GameSpeedChangedEventArgs e)
        {
            var handler = GameSpeedChanged;
            handler?.Invoke(this, e);
        }

        #endregion
    }

    #region Event Arguments
    
    public class SeasonChangedEventArgs : EventArgs
    {
        public uint Day { get; set; }
        public Season PreviousSeason { get; set; }
        public Season CurrentSeason { get; set; }
    }

    public class DayChangedEventArgs : EventArgs
    {
        public uint Yesterday { get; set; }
        public uint Today { get; set; }
    }

    public class TimeChangedEventArgs : EventArgs
    {
        public uint PreviousTime { get; set; }
        public uint CurrentTime { get; set; }
    }

    public class PauseStateChangedEventArgs : EventArgs
    {
        public bool PreviousPauseState { get; set; }
    }

    public class GameSpeedChangedEventArgs : EventArgs
    {
        public float PreviousGameSpeed { get; set; }
        public float NewGameSpeed { get; set; }
    }
    
    #endregion
}
