using System;

namespace DProject.Type
{
    public class Message
    {
        private readonly string _messageText;
        private readonly DateTime _dateTime;
        private int _timeLeft;

        public Message(string messageText)
        {
            _messageText = messageText;
            _dateTime = DateTime.Now;
            _timeLeft = 300;
        }

        public string GetMessage()
        {
            return _messageText;
        }

        public DateTime GetDateTime()
        {
            return _dateTime;
        }

        public void TickLifeTime()
        {
            _timeLeft--;
        }

        public int GetLifeTime()
        {
            return _timeLeft;
        }
    }
}