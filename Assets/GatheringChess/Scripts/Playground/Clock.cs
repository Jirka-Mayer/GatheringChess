using System;
using UnityEngine;

namespace GatheringChess.Playground
{
    public class Clock
    {
        private bool meRunning;
        private float myStartAt;
        private float myRemainingAtStart;

        public float MyDisplayTime => meRunning
            ? myRemainingAtStart - (
                  Time.realtimeSinceStartup - myStartAt
              )
            : myRemainingAtStart;

        private bool opponentRunning;
        private float opponentStartAt;
        private float opponentRemainingAtStart;
        
        public float OpponentDisplayTime => opponentRunning
            ? opponentRemainingAtStart - (
                  Time.realtimeSinceStartup - opponentStartAt
              )
            : opponentRemainingAtStart;
        
        public Clock(float seconds)
        {
            myRemainingAtStart = seconds;
            opponentRemainingAtStart = seconds;

            meRunning = false;
            opponentRunning = false;
        }

        public static string FormatTime(float time)
        {
            if (time < 0f)
                time = 0f;
            
            int seconds = (int)time;
            int minutes = seconds / 60;
            seconds %= 60;

            return $"{minutes}:{seconds:D2}";
        }

        public bool IsTimeOverForMe() => MyDisplayTime <= 0f;
        
        public void StartOpponent()
        {
            if (opponentRunning)
                throw new InvalidOperationException();

            opponentRunning = true;
            opponentStartAt = Time.realtimeSinceStartup;
        }

        public void StopOpponent(float actualDuration)
        {
            if (!opponentRunning)
                throw new InvalidOperationException();

            opponentRemainingAtStart -= actualDuration;
            opponentRunning = false;
        }

        public void StartMe()
        {
            if (meRunning)
                throw new InvalidOperationException();

            meRunning = true;
            myStartAt = Time.realtimeSinceStartup;
        }

        public float StopMe()
        {
            if (!meRunning)
                throw new InvalidOperationException();
            
            var currentRemaining = MyDisplayTime;
            var duration = myRemainingAtStart - currentRemaining;
            myRemainingAtStart = currentRemaining;
            
            meRunning = false;
            
            return duration;
        }
    }
}