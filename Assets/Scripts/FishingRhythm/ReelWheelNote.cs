using System.Runtime.CompilerServices;
using UnityEngine;

public enum NoteType
{
    Tap,
    Hold,
    Altkey,
    Skip
}

public class ReelWheelNote : MonoBehaviour
{
    public float travelTime;
    float currentLifeTime;

    public bool justSpawned = true; // grace period flag

    public NoteType noteType = NoteType.Tap;
    public float holdDuration = 0.35f; // only used when noteType == Hold

    public bool isVisualOnly = false; // true for the tail marker / bar - not hittable, not registered

    public GameObject holdTail; // only set on the head note, when noteType == Hold
    public GameObject holdBar;  // only set on the head note, when noteType == Hold

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
        else if (justSpawned == true && currentLifeTime >= travelTime * 0.1f)
        {
            justSpawned = false;
        }
    }

    void OnDestroy()
    {
        // Take the tail and bar down with the head, regardless of why the head died.
        if (holdTail != null) Destroy(holdTail);
        if (holdBar != null) Destroy(holdBar);

        if (isVisualOnly) return; // never registered, nothing to clean up

        if (NoteHitManager.Instance != null)
        {
            if (NoteHitManager.Instance.IsRegistered(gameObject))
                NoteHitManager.Instance.UnregisterNote(gameObject);
        }
    }
}