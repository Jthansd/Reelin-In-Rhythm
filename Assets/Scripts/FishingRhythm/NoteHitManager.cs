using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteHitManager : MonoBehaviour
{
    private List<List<NoteTiming>> spawnTimes = new List<List<NoteTiming>>();
    private int nextNoteIndex = 0;
    private float spawnLeadTime;
    private float hitWindow = 15f;
    private List<GameObject> activeNotes = new List<GameObject>();

    [SerializeField] private ReelWheel reelWheel;
    [SerializeField] private RectTransform wheelTransform;

    private static readonly string[] NOTE_RESULTS = { "HIT", "EARLY", "LATE", "MISSED" };

    public static NoteHitManager Instance { get; internal set; }

    // Hold-note tracking state
    private bool isHolding = false;
    private float holdStartTime = 0f;
    private GameObject holdingNote = null;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnLeadTime = 2f * (60f / reelWheel.bpm);
    }

    void Update()
    {
        HandleFrontNote();
    }

    public void SpawnNotesOnTime()
    {
        if (!reelWheel.IsPlaying())
            return;

        List<NoteTiming> easyTimings = spawnTimes[0];

        if (nextNoteIndex >= easyTimings.Count)
            return;

        float currentTime = MusicController.Instance.GetCurrentMusicTime();
        NoteTiming nextTiming = easyTimings[nextNoteIndex];

        if (currentTime >= nextTiming.hitTime - spawnLeadTime)
        {
            reelWheel.SpawnSingleNote(nextTiming);
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

    private void HandleFrontNote()
    {
        if (activeNotes.Count == 0) return;

        GameObject front = activeNotes[0];
        if (front == null)
        {
            activeNotes.RemoveAt(0);
            return;
        }

        ReelWheelNote noteComp = front.GetComponent<ReelWheelNote>();
        if (noteComp != null && noteComp.justSpawned)
            return;

        switch (noteComp.noteType)
        {
            case NoteType.Tap:
                CheckTapInput(front);
                break;
            case NoteType.Hold:
                CheckHoldInput(front, noteComp);
                break;
        }
    }

    public void CheckInput()
    {
        // Tap/Hold handling now lives in HandleFrontNote(), called every frame from this script's own Update().
    }

    private void CheckTapInput(GameObject note)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            string result = IsNoteInHitWindow(note);
            if (result == NOTE_RESULTS[0])
            {
                Debug.Log(NOTE_RESULTS[0]);
                reelWheel.UpdateFishDistance(true);
            }
            else
            {
                Debug.Log("MISS!");
                reelWheel.UpdateFishDistance(false);
            }

            UnregisterNote(note);
            Destroy(note);
            return;
        }

        if (IsTooLate(note))
        {
            Debug.Log("TOO LATE!");
            reelWheel.UpdateFishDistance(false);
            UnregisterNote(note);
            Destroy(note);
        }
    }

    private void CheckHoldInput(GameObject note, ReelWheelNote noteComp)
    {
        bool inWindow = IsNoteInHitWindow(note) == NOTE_RESULTS[0];
        bool spacePressed = Keyboard.current.spaceKey.isPressed;

        if (!isHolding)
        {
            if (inWindow && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                isHolding = true;
                holdStartTime = Time.time;
                holdingNote = note;
            }
            else if (IsTooLate(note))
            {
                Debug.Log("MISSED HOLD (never started)");
                reelWheel.UpdateFishDistance(false);
                UnregisterNote(note);
                Destroy(note);
            }
        }
        else
        {
            if (!spacePressed)
            {
                Debug.Log("MISSED HOLD (released early)");
                reelWheel.UpdateFishDistance(false);
                EndHold(note);
            }
            else if (Time.time - holdStartTime >= noteComp.holdDuration)
            {
                Debug.Log("HOLD SUCCESS");
                reelWheel.UpdateFishDistance(true);
                EndHold(note);
            }
        }
    }

    private void EndHold(GameObject note)
    {
        isHolding = false;
        holdingNote = null;
        UnregisterNote(note);
        Destroy(note);
    }

    private float GetNoteAngle(GameObject note)
    {
        RectTransform rt = note.GetComponent<RectTransform>();
        Vector2 localPos = rt.localPosition;

        float localAngle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
        if (localAngle < 0) localAngle += 360f;

        float wheelAngle = wheelTransform.localRotation.eulerAngles.z;
        float worldAngle = (localAngle + wheelAngle) % 360f;
        return worldAngle;
    }

    private string IsNoteInHitWindow(GameObject note)
    {
        float noteAngle = GetNoteAngle(note);
        float delta = Mathf.Abs(Mathf.DeltaAngle(noteAngle, 0f));

        if (delta <= hitWindow)
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
        return delta > hitWindow;
    }

    public bool IsRegistered(GameObject note)
    {
        return activeNotes.Contains(note);
    }

    public void ResetNotes()
    {
        nextNoteIndex = 0;
        isHolding = false;
        holdingNote = null;
        foreach (var note in activeNotes)
            if (note != null) Destroy(note);
        activeNotes.Clear();
    }
}