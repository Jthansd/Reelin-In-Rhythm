using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "Scriptable Objects/Fish")]
public class Fish : ScriptableObject
{
    [SerializeField] private string fishName;
    [SerializeField] private string description;

    public string FishName => fishName;
    public string Description => description;
}
