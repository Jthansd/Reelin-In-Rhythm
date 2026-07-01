using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteHitManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<List<float>> spawnTimes = new List<List<float>>();
    private int nextNoteIndex = 0;
    private float spawnLeadTime;
    private float hitWindow = 15f;
    private List<GameObject> activeNotes = new List<GameObject>();

    [SerializeField] private ReelWheel reelWheel;
    [SerializeField] private RectTransform wheelTransform;

    private static readonly string[] NOTE_RESULTS = { "HIT", "EARLY", "LATE", "MISSED" };

    public static NoteHitManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnLeadTime = 2f * (60f / reelWheel.bpm);
    }

    // Update is called once per frame
    void Update()
    {
        checkIsLate();
    }

    

    public void SpawnNotesOnTime()
    {
        if (!reelWheel.IsPlaying())
            return;

    
        //responsible for spawning notes on time based on the list of spawn times

        List<float> easyTimes = spawnTimes[0];

        if (nextNoteIndex >= easyTimes.Count)
            return;

        // Get the current music time from the MusicController
        float currentTime = MusicController.Instance.GetCurrentMusicTime();
        // Get the next note's hit time
        float hitTime = easyTimes[nextNoteIndex];

        //Spawn the note if the current time is within the lead time of the hit time
        if (currentTime >= hitTime - spawnLeadTime)
        {
            //spawn note
            reelWheel.SpawnSingleNote();
            //move on to the next note
            nextNoteIndex++;
        }
        
    }

    public void GetSpawnTimes()
    {
        spawnTimes.Clear();
        spawnTimes.Add(MusicController.Instance.GetNoteTimingsForDifficulty("easy"));
        spawnTimes.Add(MusicController.Instance.GetNoteTimingsForDifficulty("medium"));
        spawnTimes.Add(MusicController.Instance.GetNoteTimingsForDifficulty("hard"));
    }

    public void RegisterNote(GameObject note)
    {
        if (note == null) return;
        activeNotes.Add(note);
    }

    public void UnregisterNote(GameObject note)
    {
        if (note == null) return;
        activeNotes.Remove(note);
    }

    public void CheckInput()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;

        if (activeNotes.Count == 0)
            return;

        GameObject nextNote = activeNotes[0];

        if (IsNoteInHitWindow(nextNote) == NOTE_RESULTS[0])
        {
            Debug.Log(NOTE_RESULTS[0]);

            // award points, reduce fish distance, etc.

            reelWheel.UpdateFishDistance(true);
        }
        else
        {
            Debug.Log("MISS!");
            // apply miss penalty
            reelWheel.UpdateFishDistance(false);
        }

        UnregisterNote(nextNote);
        Destroy(nextNote);
    }

    private float GetNoteAngle(GameObject note)
    {
        RectTransform rt = note.GetComponent<RectTransform>();
        Vector2 localPos = rt.localPosition;

        float localAngle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
        if (localAngle < 0) localAngle += 360f;

        // Add the wheel's current Z rotation to get the true angle
        float wheelAngle = wheelTransform.localRotation.eulerAngles.z;
        float worldAngle = (localAngle + wheelAngle) % 360f;
        //Debug.Log($"Note Angle: {localAngle}, Wheel Angle: {wheelAngle}, World Angle: {worldAngle}");
        return worldAngle;
    }


    private string IsNoteInHitWindow(GameObject note)
    {
        float noteAngle = GetNoteAngle(note);

        // Hit zone is at 0 degrees
        float delta = Mathf.Abs(Mathf.DeltaAngle(noteAngle, 0f));

        if(delta <= hitWindow)
            return NOTE_RESULTS[0];
        else if (delta < 180f)
            return NOTE_RESULTS[1];
        else
            return NOTE_RESULTS[2];
    }

    private bool IsTooLate(GameObject note)
    {
        float noteAngle = GetNoteAngle(note);
        float delta = Mathf.DeltaAngle(noteAngle, 0f);
        // Clockwise travel means the note passes 0° and angle increases past hitWindow
        return delta > hitWindow;
    }

    private void checkIsLate()
    {
        if (activeNotes.Count == 0) return;

        if (activeNotes[0] == null)
        {
            activeNotes.RemoveAt(0);
            return;
        }

        // Don't check notes that spawned this frame
        ReelWheelNote noteComp = activeNotes[0].GetComponent<ReelWheelNote>();
        if (noteComp != null && noteComp.justSpawned)
            return;

        if (IsTooLate(activeNotes[0]))
        {
            Debug.Log("TOO LATE!");
            reelWheel.UpdateFishDistance(false);
            UnregisterNote(activeNotes[0]);
        }
    }

    public bool IsRegistered(GameObject note)
    {
        return activeNotes.Contains(note);
    }

    public void ResetNotes()
    {
        nextNoteIndex = 0;
        foreach (var note in activeNotes)
            if (note != null) Destroy(note);
        activeNotes.Clear();
    }

}
