using UnityEngine;

public class BonusReel : MonoBehaviour
{
    private int bonusHits = 0;

    public void Reset()
    {
        bonusHits = 0;
    }

    public void BonusHit()
    {
        bonusHits++;
    }

    public int GetBonusHits()
    {
        return bonusHits;
    }
}
