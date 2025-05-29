# Playbox timer


> The timer is necessary to facilitate interaction with time variables and remove their low-level use.
> 
> PlayboxTimer : [PlayboxTimer Documentation](@ref Utils.Timer.PlayboxTimer)
> 
> Before you start working with the timer, you need to create an instance of the PlayboxTimer class :
> 
>    <code data-lang="csharp">PlayboxTimer timer = new PlayboxTimer();</code>
> 
> We can then optionally specify the start time of the timer
> 
>    <code data-lang="csharp">timer.initialTime = 5.0f;</code>
> 
>The PlayboxTimer class has many [callbacks](@ref Utils.Timer.PlayboxTimer.OnTimerStart) to make it easy to use. 
> [See here for details](@ref Utils.Timer.PlayboxTimer.OnTimerStart).
> But now we will use the [OnTimeOut](@ref Utils.Timer.PlayboxTimer.OnTimeOut) event.
> 
>    <code data-lang="csharp">timer.OnTimeOut += (){ Debug.Log("TimeOut"); }</code> 
> 
> This event will be triggered only when the timer time expires.
> 
> But to start the timer, you must call the [timer.Start();](@ref Utils.Timer.PlayboxTimer.Start) method;
> 
>    <code data-lang="csharp">timer.Start();</code>
> 