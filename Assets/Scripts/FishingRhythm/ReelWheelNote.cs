using System.Runtime.CompilerServices;
using UnityEngine;

public class ReelWheelNote : MonoBehaviour
{
    public float travelTime;
    float currentLifeTime;

    public bool justSpawned = true; // grace period flag

    void Start()
    {
        currentLifeTime = 0f;
    }

    void Update()
    {
        
        currentLifeTime += Time.deltaTime;

        if (currentLifeTime >= travelTime)
        {
            Destroy(gameObject);
        }
        else if(justSpawned == true && currentLifeTime >= travelTime * 0.1f)
        {
            justSpawned = false; 
        }
    }

    void OnDestroy()
    {
        if (NoteHitManager.Instance != null)
        {
            // Only unregister if it's still in the list
            if (NoteHitManager.Instance.IsRegistered(gameObject))
                NoteHitManager.Instance.UnregisterNote(gameObject);
        }
    }
}
