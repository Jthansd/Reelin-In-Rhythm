using UnityEngine;

public class PurchaseUnlockCondition : IUnlockCondition
{
    private readonly string requiredPurchaseItemId;
    private readonly PlayerEquipment playerEquipment;

    public PurchaseUnlockCondition(string requiredPurchaseItemId, PlayerEquipment playerEquipment)
    {
        this.requiredPurchaseItemId = requiredPurchaseItemId;
        this.playerEquipment = playerEquipment;
    }

    public bool IsUnlocked()
    {
        return playerEquipment.Owns(requiredPurchaseItemId);
    }
    
}
