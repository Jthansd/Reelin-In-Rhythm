public enum FishingState
{
    Idle,           // not fishing, casting is allowed
    WaitingForBite, // bobber in water, checking for a bite; early reel-in allowed here
    Hooked,         // fish hooked, about to start reeling
    Reeling,        // reel wheel minigame active
}