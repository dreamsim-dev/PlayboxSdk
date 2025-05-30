using System;

namespace Utils.Timer
{
    /// <summary>
    /// Represents a timer that provides events for various timer states (start, stop, pause, resume, timeout, elapsed and remaining time).
    /// </summary>
    public class PlayboxTimer
    {
        /// <summary>
        /// Occurs when the timer starts.
        /// </summary>
        public event Action OnTimerStart;

        /// <summary>
        /// Occurs when the timer is forcibly stopped.
        /// </summary>
        public event Action OnTimerStopped;

        /// <summary>
        /// Occurs when the timer is paused.
        /// </summary>
        public event Action OnTimerPaused;

        /// <summary>
        /// Occurs when the timer is resumed from pause.
        /// </summary>
        public event Action OnTimerResumed;

        /// <summary>
        /// Occurs when the timer reaches the timeout (time runs out).
        /// </summary>
        public event Action OnTimeOut;

        /// <summary>
        /// Occurs regularly to provide the elapsed time since the timer started.
        /// </summary>
        public event Action<float> OnTimeElapsed;

        /// <summary>
        /// Occurs regularly to provide the remaining time until the timeout.
        /// </summary>
        public event Action<float> OnTimeRemaining;

        /// <summary>
        /// Gets or sets the initial countdown time of the timer in seconds.
        /// </summary>
        public float initialTime { get; set; } = 5;
        
        private float timeElapsed;
        private float timeRemaining;

        private bool isPaused;
        
        /// <summary>
        /// Starts the timer from the beginning, triggering the <see cref="OnTimerStart"/> event.
        /// </summary>
        public void Start()
        {
            timeElapsed = 0;
            timeRemaining = initialTime;
            
            OnTimerStart?.Invoke();
        }
        
        /// <summary>
        /// Forces the timer to stop, triggering the <see cref="OnTimerStopped"/> event and resetting the timer state.
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
        /// Restarts the timer by stopping and then starting it.
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Pauses the timer, triggering the <see cref="OnTimerPaused"/> event and reporting current time state.
        /// </summary>
        public void Pause()
        {
            isPaused = true;
            
            OnTimerPaused?.Invoke();
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }

        /// <summary>
        /// Resumes the timer from the paused state, triggering the <see cref="OnTimerResumed"/> event and updating time state.
        /// </summary>
        public void Resume()
        {
            isPaused = false;
            
            OnTimerResumed?.Invoke();
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }

        /// <summary>
        /// Updates the timer by advancing it with the given delta time. Fires <see cref="OnTimeElapsed"/>, <see cref="OnTimeRemaining"/>, and potentially <see cref="OnTimeOut"/>.
        /// </summary>
        /// <param name="deltaTime">The amount of time to advance the timer, typically <c>Time.deltaTime</c>.</param>
        public void Update(float deltaTime)
        {
            if (isPaused)
                return;
            
            timeElapsed += deltaTime;
            timeRemaining -= deltaTime;
            
            if (timeRemaining < 0)
                OnTimeOut?.Invoke();
            
            OnTimeElapsed?.Invoke(timeElapsed);
            OnTimeRemaining?.Invoke(timeRemaining);
        }
    }
}
