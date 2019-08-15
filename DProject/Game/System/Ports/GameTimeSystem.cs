using System;
using DProject.Game.Component.Ports;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace DProject.Manager.System.Ports
{
    public class GameTimeSystem : EntityProcessingSystem
    {
        public const uint TicksInADay = 720;
        public const uint DaysInASeason = 30;
        
        private ComponentMapper<GameTimeComponent> _timeMapper;
        
        public GameTimeSystem() : base(Aspect.All(typeof(GameTimeComponent))) { }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _timeMapper = mapperService.GetMapper<GameTimeComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var time = _timeMapper.Get(entityId);

            if (time.Paused) return;
            
            time.CurrentSecond += gameTime.ElapsedGameTime.TotalSeconds * time.GameSpeed;
            
            if (time.CurrentSecond >= 1.0d)
            {
                Tick(time, Convert.ToUInt32(Math.Floor(time.CurrentSecond)));
                
                time.CurrentSecond = 0.0d;
            }
        }
        
        private static void Tick(GameTimeComponent time, uint amount = 1)
        {
            var previousTime = time.CurrentTime;
            
            time.CurrentTime += amount;

            if (time.CurrentTime >= TicksInADay)
            {
                time.CurrentTime = 0; 
                SetNextDay(time);
            }
            
            var args = new TimeChangedEventArgs
            {
                PreviousTime = previousTime,
                CurrentTime = time.CurrentTime
            };
            
            time.OnTimeChanged(args);
            
            Console.WriteLine(time.CurrentSeason + ", " + time.CurrentDay + ", " + time.CurrentTime);
        }
        
        private static void SetNextDay(GameTimeComponent time)
        {
            var args = new DayChangedEventArgs()
            {
                Yesterday = time.CurrentDay,
                Today = ++time.CurrentDay
            };
            
            time.OnDayChanged(args);
            
            time.CurrentSeason = DayToSeason(time);
        }
        
        private static Season DayToSeason(GameTimeComponent time)
        {
            var newSeason = (Season) (time.CurrentDay / DaysInASeason % DaysInASeason % (int) Season.Count);

            if (newSeason != time.CurrentSeason)
            {
                var args = new SeasonChangedEventArgs
                {
                    Day = time.CurrentDay,
                    PreviousSeason = time.CurrentSeason,
                    CurrentSeason = newSeason
                };
                
                time.OnSeasonChanged(args);
            }

            return newSeason;
        }

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
}
