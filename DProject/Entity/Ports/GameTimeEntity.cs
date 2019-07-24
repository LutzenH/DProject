using System;
using DProject.Type.Enum;
using Microsoft.Xna.Framework;
using IUpdateable = DProject.Entity.Interface.IUpdateable;

namespace DProject.Entity.Ports
{
    public class GameTimeEntity  : AbstractEntity, IUpdateable
    {
        private const uint TicksInADay = 720;
        private const uint DaysInASeason = 30;
        
        public bool IsPaused { get; set; }
        public float GameSpeed { get; set; }

        public uint CurrentDay { get; set; }
        public uint CurrentTime { get; set; }

        private double _currentSecond;

        private Seasons _currentSeason;

        public GameTimeEntity() : base(Vector3.Zero, Quaternion.Identity, Vector3.Zero)
        {
            CurrentDay = 0;
            CurrentTime = 0;
            IsPaused = false;
            GameSpeed = 1.0f;

            _currentSecond = 0f;
            _currentSeason = Seasons.Spring;
        }
        
        public void Update(GameTime gameTime)
        {
            if (IsPaused) return;
            
            _currentSecond += gameTime.ElapsedGameTime.TotalSeconds * GameSpeed;

            if (_currentSecond >= 1.0d)
            {
                _currentSecond = 0.0d;
                Tick();
            }
            
            Console.WriteLine("Season: " + GetCurrentSeason()
                                         + ", Day: " + CurrentDay
                                         + ", Time: " + TimeToTimeString(CurrentTime, _currentSecond));
        }

        public void Tick(uint amount = 1)
        {
            CurrentTime += amount;

            if (CurrentTime >= TicksInADay)
            {
                CurrentTime = 0;
                CurrentDay++;
                _currentSeason = DayToSeason(CurrentDay);
            }
        }

        public Seasons GetCurrentSeason()
        {
            return _currentSeason;
        }

        private static Seasons DayToSeason(uint currentDay)
        {
            return (Seasons) (currentDay / DaysInASeason % DaysInASeason);
        }

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
    }
}