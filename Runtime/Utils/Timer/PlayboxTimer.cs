using System;

namespace Utils.Timer
{
    /// <summary>
    /// Timer with events
    /// </summary>
    public class PlayboxTimer
    {
        /// <summary>
        /// Called when the timer starts
        /// </summary>
        public event Action OnTimerStart;
        /// <summary>
        /// Called when the timer has been forced to stop
        /// </summary>
        public event Action OnTimerStopped;
        /// <summary>
        /// Called when the timer has been paused
        /// </summary>
        public event Action OnTimerPaused;
        /// <summary>
        /// Called when the timer has been unpaused
        /// </summary>
        public event Action OnTimerResumed;
        /// <summary>
        /// Called when the time is up
        /// </summary>
        public event Action OnTimeOut;
        /// <summary>
        /// Refreshes and returns the time elapsed from the beginning
        /// </summary>
        public event Action<float> OnTimeElapsed;
        /// <summary>
        /// Returns the time remaining from the start
        /// </summary>
        public event Action<float> OnTimeRemaining;
        /// <summary>
        /// The field responsible for the initial time
        /// </summary>

        public float initialTime { get; set; } = 5;
        
        private float timeElapsed;
        private float timeRemaining;

        private bool isPaused;
        
        /// <summary>
        /// starts the timer from the beginning
        /// </summary>
        public void Start()
        {
            timeElapsed = 0;
            timeRemaining = initialTime;
            
            OnTimerStart?.Invoke();
        }
        
        /// <summary>
        /// Forced stop of the timer
        /// </summary>
        public void Stop()
        {
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
            
            timeElapsed = 0;
            timeRemaining = initialTime;
            
            OnTimerStopped?.Invoke();
        }
        /// <summary>
        /// Restarting the timer
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }
        /// <summary>
        /// Setting the timer to pause
        /// </summary>
        public void Pause()
        {
            isPaused = true;
            
            OnTimerPaused?.Invoke();
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }
        /// <summary>
        /// Unpausing the timer
        /// </summary>
        public void Resume()
        {
            isPaused = false;
            
            OnTimerResumed?.Invoke();
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }
        /// <summary>
        /// Method for updating the timer
        /// </summary>
        /// <param name="deltaTime">Delta time from <code>Time.deltaTime</code></param>
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