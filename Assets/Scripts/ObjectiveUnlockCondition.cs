using UnityEngine;

public class ObjectiveUnlockCondition : IUnlockCondition
{
    private readonly string objectiveId;

    public ObjectiveUnlockCondition(string objectiveId)
    {
        this.objectiveId = objectiveId;
    }

    public bool IsUnlocked()
    {
        return ObjectiveManager.Instance.IsCompleted(objectiveId);
    }
}
