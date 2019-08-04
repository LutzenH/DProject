using System;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Ports
{
    public class GameTimeEntity  : AbstractEntity, IUpdateable
    {
        public const uint TicksInADay = 720;
        public const uint DaysInASeason = 30;
        
        public uint CurrentDay { get; private set; }
        public uint CurrentTime { get; private set; }
        public Season CurrentSeason { get; private set; }

        private double _currentSecond;
        private float _gameSpeed;
        private bool _paused;

        public GameTimeEntity() : base(Vector3.Zero, Quaternion.Identity, Vector3.Zero)
        {
            CurrentDay = 0;
            CurrentTime = 0;
            Paused = false;
            GameSpeed = 1.0f;

            _currentSecond = 0f;
            CurrentSeason = Season.Vernal;
        }
        
        public void Update(GameTime gameTime)
        {
            if (Paused) return;
            
            _currentSecond += gameTime.ElapsedGameTime.TotalSeconds * GameSpeed;

            if (_currentSecond >= 1.0d)
            {
                Tick( Convert.ToUInt32(Math.Floor(_currentSecond)));
                _currentSecond = 0.0d;
            }
        }

        public void Tick(uint amount = 1)
        {
            var previousTime = CurrentTime;
            
            CurrentTime += amount;

            if (CurrentTime >= TicksInADay)
            {
                CurrentTime = 0;
                SetNextDay();
            }
            
            var args = new TimeChangedEventArgs
            {
                PreviousTime = previousTime,
                CurrentTime = CurrentTime
            };
            
            OnTimeChanged(args);
        }

        public double GetRelativeTime()
        {
            return (CurrentTime + _currentSecond) / TicksInADay;
        }

        private void SetNextDay()
        {
            var args = new DayChangedEventArgs()
            {
                Yesterday = CurrentDay,
                Today = ++CurrentDay
            };
            
            OnDayChanged(args);
            
            CurrentSeason = DayToSeason(CurrentDay);
        }
        
        private Season DayToSeason(uint currentDay)
        {
            var newSeason = (Season) (currentDay / DaysInASeason % DaysInASeason % (int) Season.Count);

            if (newSeason != CurrentSeason)
            {
                var args = new SeasonChangedEventArgs
                {
                    Day = currentDay,
                    PreviousSeason = CurrentSeason,
                    CurrentSeason = newSeason
                };
                
                OnSeasonChanged(args);
            }

            return newSeason;
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
        protected virtual void OnSeasonChanged(SeasonChangedEventArgs e)
        {
            var handler = SeasonChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<DayChangedEventArgs> DayChanged; 
        protected virtual void OnDayChanged(DayChangedEventArgs e)
        {
            var handler = DayChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<TimeChangedEventArgs> TimeChanged;
        protected virtual void OnTimeChanged(TimeChangedEventArgs e)
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

        #region Utilities

        private static string TimeToTimeString(uint currentTime, double currentSecond)
        {
            var (hour, minute) = GetTimeAsSiTime(currentTime, currentSecond);

            return hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0');
        }

        private static (uint, uint) GetTimeAsSiTime(uint currentTime, double currentSecond)
        {
            var currentTimeInRealWorldMinutes =
                (uint) Math.Floor((currentTime + currentSecond) / TicksInADay * 1440); //1440 minutes in a real day

            var hour = currentTimeInRealWorldMinutes / 60;
            var minute = currentTimeInRealWorldMinutes % 60;

            return (hour, minute);
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