using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class NoteHitManager : MonoBehaviour
{
    private List<NoteTiming> spawnTimes; 
    private int nextNoteIndex = 0;
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

    private KeyControl alternativeKey;

    [SerializeField] private GameObject hitFeedbackPrefab;
    [SerializeField] private RectTransform feedbackAnchor;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Color HitColor = Color.white;
    [SerializeField] private Color MissColor = Color.white;
    [SerializeField] private Color LateColor = Color.white;
    [SerializeField] private Color EarlyColor = Color.white;


    private void SpawnFeedback(string message, Color color)
    {
        GameObject popup = Instantiate(hitFeedbackPrefab, canvasTransform);
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = feedbackAnchor.anchoredPosition; // same space, since both share canvasTransform as parent

        HitFeedbackPopup feedback = popup.GetComponent<HitFeedbackPopup>();
        feedback.Show(message, color);
    }


    private void Awake()
    {
        Instance = this;
        alternativeKey = Keyboard.current.leftArrowKey;

    }

    

    private float GetSpawnLeadTime()
    {
        return 2f * (60f / reelWheel.bpm);
    }

    void Update()
    {
        HandleFrontNote();
    }

    public void SpawnNotesOnTime()
    {
        if (!reelWheel.IsPlaying())
            return;

        List<NoteTiming> easyTimings = spawnTimes;

        if (nextNoteIndex >= easyTimings.Count)
            return;

        float currentTime = MusicController.Instance.GetCurrentMusicTime();
        NoteTiming nextTiming = easyTimings[nextNoteIndex];

        if (currentTime >= nextTiming.hitTime - GetSpawnLeadTime())
        {
            reelWheel.SpawnSingleNote(nextTiming);
            nextNoteIndex++;
        }
    }

    public void GetSpawnTimes()
    {

        spawnTimes = MusicController.Instance.GetNoteTimings();
        
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
            case NoteType.Altkey:
            // Handle Altkey input if needed
                CheckTapInput(front);
                break;
            case NoteType.Skip:
                // Handle Skip input if needed
                CheckSkipInput(front);
                break;
        }


    }


    public ReelWheelNote GetFrontNote()
    {
        return activeNotes[0].GetComponent<ReelWheelNote>();
    }

    

    public void CheckInput()
    {
        // Tap/Hold handling now lives in HandleFrontNote(), called every frame from this script's own Update().
    }

    private void CheckSkipInput(GameObject note)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("ACTED ON SKIP!");
            SpawnFeedback("MISS", MissColor);
            reelWheel.UpdateFishDistance(false);
            UnregisterNote(note);
            Destroy(note);
            return;
        }

        if(IsTooLate(note))
        {
            Debug.Log("SKIP SUCCESS!");
            SpawnFeedback("HIT", HitColor);
            reelWheel.UpdateFishDistance(true);
            UnregisterNote(note);
            Destroy(note);
        }
    }

    private void CheckTapInput(GameObject note)
    {
        NoteType noteType = note.GetComponentInChildren<ReelWheelNote>().noteType;
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if(noteType == NoteType.Altkey)
            {
                Debug.Log("Wrong Key!");
                SpawnFeedback("MISS", MissColor);
                reelWheel.UpdateFishDistance(false);
            }
            else
            {
                string result = IsNoteInHitWindow(note);
                if (result == NOTE_RESULTS[0])
                {
                    Debug.Log(NOTE_RESULTS[0]);
                    SpawnFeedback("HIT", HitColor);
                    reelWheel.UpdateFishDistance(true);
                }
                else
                {
                    Debug.Log("MISS!");
                    SpawnFeedback("MISS", MissColor);
                    reelWheel.UpdateFishDistance(false);
                }
            }

            UnregisterNote(note);
            Destroy(note);
            return;

        }
        else if (alternativeKey.wasPressedThisFrame)
        {
            if (noteType == NoteType.Tap)
            {
                Debug.Log("Wrong Key!");
                SpawnFeedback("MISS", MissColor);
                reelWheel.UpdateFishDistance(false);
            }
            else
            {
                string result = IsNoteInHitWindow(note);
                if (result == NOTE_RESULTS[0])
                {
                    Debug.Log(NOTE_RESULTS[0]);
                    SpawnFeedback("HIT", HitColor);
                    reelWheel.UpdateFishDistance(true);
                }
                else
                {
                    Debug.Log("MISS!");
                    SpawnFeedback("MISS", MissColor);
                    reelWheel.UpdateFishDistance(false);
                }
            }
            UnregisterNote(note);
            Destroy(note);
            return;
        }


        if (IsTooLate(note))
        {
            Debug.Log("TOO LATE!");
            SpawnFeedback("LATE", LateColor);
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
                SpawnFeedback("MISS", MissColor);
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
                SpawnFeedback("EARLY", EarlyColor);
                reelWheel.UpdateFishDistance(false);
                EndHold(note);
            }
            else if (Time.time - holdStartTime >= noteComp.holdDuration)
            {
                Debug.Log("HOLD SUCCESS");
                SpawnFeedback("HIT", HitColor);
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