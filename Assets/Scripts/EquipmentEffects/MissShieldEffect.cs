public class MissShieldEffect : IEquipmentEffect
{
    private readonly int maxShields;
    private int shieldsRemaining;

    public MissShieldEffect(int maxShields)
    {
        this.maxShields = maxShields;
        shieldsRemaining = maxShields;
    }

    public float OnNoteMissed(FishingController context, float currentPenalty)
    {
        if (shieldsRemaining <= 0) return currentPenalty; // no shields left, penalty applies normally

        shieldsRemaining--;
        return 0f; // absorbed - no penalty this miss
    }

    public void ResetForNewEncounter()
    {
        shieldsRemaining = maxShields;
    }
}