using System;

namespace Utils.Timer
{
    public class PlayboxTimer
    {
        public event Action OnTimerStart;
        public event Action OnTimerStopped;
        public event Action OnTimerPaused;
        public event Action OnTimerResumed;
        public event Action OnTimeOut;
        public event Action<float> OnTimeElapsed;
        public event Action<float> OnTimeRemaining;

        public float initialTime { get; set; } = 5;
        
        private float timeElapsed;
        private float timeRemaining;

        private bool isPaused;
        
        public void Start()
        {
            timeElapsed = 0;
            timeRemaining = initialTime;
            
            OnTimerStart?.Invoke();
        }
        
        public void Stop()
        {
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
            
            timeElapsed = 0;
            timeRemaining = initialTime;
            
            OnTimerStopped?.Invoke();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            isPaused = true;
            
            OnTimerPaused?.Invoke();
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }

        public void Resume()
        {
            isPaused = false;
            
            OnTimerResumed?.Invoke();
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }
        
        public void Update(float deltaTime)
        {
            if (isPaused)
                return;
            
            timeElapsed += deltaTime;
            timeRemaining -= deltaTime;
            
            if(timeRemaining < 0)
                OnTimeOut?.Invoke();
            
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }
    }
}