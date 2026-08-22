using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReelWheel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject reelWheelUI; // The parent UI object for the reel wheel
    public RectTransform Spawnpoint;
    public RectTransform wheel;     // The circular track (optional)
    public GameObject notePrefab;
    public float orbitSpeed = 180f; // degrees per second
    public float hitWindow = 15f;   // degrees around the top

    private float angle = 0f;

    private bool isRunning = false; // internal only - is the wheel actively spinning right now

    [Header("Music Sync")]
    public float bpm = 130f;
    public float beatsPerMeasure = 4f;
    public float rotationOffset = 0f;

    public float accumulatedAngle = 0f;

    [Header("Hold Note Visuals")]
    [SerializeField] private Color holdNoteColor = new Color(0.2f, 0.5f, 1f, 1f); // blue
    [SerializeField] private Color skipNoteColor = Color.red;
    [SerializeField] private Color altNoteColor = Color.green;
    [SerializeField] private float holdBarThickness = 8f;

    [SerializeField] private NoteHitManager noteHitManager;
    [SerializeField] private FishMeter fishMeter;
    [SerializeField] private FishingController fishingController;

    private float progressAmount = 0.1f;
    private float penaltyAmount = 0.1f;

    private float currentPauseTime;
    private static float dontPause = 999f;

    private bool playBonus;

    [SerializeField] private PlayerEquipment playerEquipment;



    // Fired exactly once per reel attempt, when the outcome is decided. bool = fish caught.
    public event Action<bool> OnReelComplete;

    public bool IsPlaying() => isRunning;
    void Start()
    {
        reelWheelUI.SetActive(false);
        Debug.Log("Reel Wheel Test Started");
        currentPauseTime = dontPause;
    }

    void Update()
    {
        if (!isRunning) return;
        if (GamePauseManager.Instance.IsPaused) return;

        RunOrbit();
        noteHitManager.SpawnNotesOnTime();
        noteHitManager.CheckInput();
        CheckTutorialPause();
    }

    private void CheckTutorialPause()
    {
        if (currentPauseTime == dontPause)
        {
            return;
        }
        else if (currentPauseTime - MusicController.Instance.GetCurrentMusicTime() <= .05f)
        {
            ReelWheelNote frontNote = noteHitManager.GetFrontNote();
            if (frontNote == null)
            {
                
                currentPauseTime = dontPause;
                return;
            }

            Debug.Log("Its time to pause");
            string noteTypeName = frontNote.noteType.ToString();
            TutorialManager.Instance.ShowIfUnseenNoteType(noteTypeName);
            currentPauseTime = dontPause;
        }
    }

    private void RunOrbit()
    {
        float secondsPerBeat = 60f / bpm;
        float secondsPerMeasure = secondsPerBeat * beatsPerMeasure;
        float degreesPerSecond = 360f / secondsPerMeasure;

        angle -= degreesPerSecond * Time.deltaTime;
        angle = (angle + 360f) % 360f;

        float rotationOffset = 90f;

        wheel.localRotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    public float GetDegreesPerSecond()
    {
        float secondsPerBeat = 60f / bpm;
        float secondsPerMeasure = secondsPerBeat * beatsPerMeasure;
        return 360f / secondsPerMeasure;
    }

    public void SpawnSingleNote(NoteTiming timing)
    {
        GameObject note = Instantiate(notePrefab, wheel);
        RectTransform noteRect = note.GetComponent<RectTransform>();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wheel,
            Spawnpoint.position,
            null,
            out localPos
        );
        noteRect.anchoredPosition = localPos;

        var noteComp = note.GetComponent<ReelWheelNote>();

        float secondsPerBeat = 60f / bpm;
        noteComp.travelTime = secondsPerBeat * 4;
        noteComp.noteType = timing.noteType;

        if (timing.noteType == NoteType.Hold)
        {

            float holdDuration = Mathf.Max(0f, timing.releaseTime - timing.hitTime);
            noteComp.holdDuration = holdDuration;

            var headImage = note.GetComponent<UnityEngine.UI.Image>();
            if (headImage != null)
                headImage.color = holdNoteColor;

            Vector2 tailLocalPos = SpawnHoldTail(localPos, noteComp.travelTime, holdDuration, out GameObject tailObj);
            GameObject bar = CreateHoldBar(localPos, tailLocalPos, noteComp.travelTime);

            noteComp.holdTail = tailObj;
            noteComp.holdBar = bar;
        }
        else if (timing.noteType == NoteType.Skip)
        {
            var headImage = note.GetComponent<UnityEngine.UI.Image>();
            if (headImage != null)
                headImage.color = skipNoteColor;
        }
        else if (timing.noteType == NoteType.Altkey)
        {
            var headImage = note.GetComponent<UnityEngine.UI.Image>();
            if (headImage != null)
                headImage.color = altNoteColor;
        }

        noteHitManager.RegisterNote(note);
      

        if(!TutorialManager.Instance.HasSeen(noteComp.noteType.ToString() + TutorialManager.Instance.NoteTypeTail) && currentPauseTime == dontPause)
        {
            Debug.Log(noteComp.noteType.ToString() + TutorialManager.Instance.NoteTypeTail + " has not been seen: popup tutorial");
            //Make this note a freebie and prevent the player from hitting the note intentionally before the popup appears
            float pauseTime = FishingMath.CalculateTutorialPause(timing.hitTime, bpm, 2);
            Debug.Log("Pause time should be " +  pauseTime);
            currentPauseTime = pauseTime;
        }
    }

       
 
    private Vector2 SpawnHoldTail(Vector2 headLocalPos, float headTravelTime, float holdDuration, out GameObject tailObject)
    {
        float offsetDegrees = GetDegreesPerSecond() * holdDuration;
        float rad = offsetDegrees * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        Vector2 tailLocalPos = new Vector2(
            headLocalPos.x * cos - headLocalPos.y * sin,
            headLocalPos.x * sin + headLocalPos.y * cos
        );

        GameObject tail = Instantiate(notePrefab, wheel);
        RectTransform tailRect = tail.GetComponent<RectTransform>();
        tailRect.anchoredPosition = tailLocalPos;

        var tailComp = tail.GetComponent<ReelWheelNote>();
        tailComp.travelTime = headTravelTime;
        tailComp.isVisualOnly = true;

        var image = tail.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = holdNoteColor;
        }

        tailObject = tail;
        return tailLocalPos;
    }

    private GameObject CreateHoldBar(Vector2 headLocalPos, Vector2 tailLocalPos, float travelTime)
    {
        Vector2 delta = tailLocalPos - headLocalPos;
        float distance = delta.magnitude;
        float angleDeg = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        Vector2 midpoint = (headLocalPos + tailLocalPos) * 0.5f;

        GameObject bar = new GameObject("HoldBar", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        bar.transform.SetParent(wheel, false);
        bar.transform.SetAsFirstSibling();

        RectTransform barRect = bar.GetComponent<RectTransform>();
        barRect.anchoredPosition = midpoint;
        barRect.sizeDelta = new Vector2(distance, holdBarThickness);
        barRect.localRotation = Quaternion.Euler(0f, 0f, angleDeg);

        var image = bar.GetComponent<UnityEngine.UI.Image>();
        image.color = holdNoteColor;

        var noteComp = bar.AddComponent<ReelWheelNote>();
        noteComp.travelTime = travelTime;
        noteComp.isVisualOnly = true;

        return bar;
    }

    public void StartReelWheel()
    {
        currentPauseTime = dontPause;
        MusicController.Instance.OnSongFinished += HandleSongFinished;
        MusicController.Instance.OnStartReelWheel();
        SetBPM(MusicController.Instance.GetCurrentBPM());
        CameraOrbit.Instance.SetLookEnabled(false);
        fishMeter.ResetProgress();
        reelWheelUI.SetActive(true);
        angle = 0f;
        noteHitManager.ResetNotes();
        noteHitManager.GetSpawnTimes();
        isRunning = true;
        MusicController.Instance.PlayMusic();
        progressAmount = fishingController.GetProgress();
        penaltyAmount = fishingController.GetMissPenalty();
        Debug.Log("Progress amount set to: " + progressAmount);
        Debug.Log("Penalty amount set to: " + penaltyAmount);
        TutorialManager.Instance.ShowIfUnseen("First_ReelWheel", "This is the Reel Wheel! Fish are attracted to music, allowing you to reel them in closer with every note you hit. But be careful! Any missed notes and the fish will pull away from you.");
    }

    private void StopReelWheel()
    {
        MusicController.Instance.OnSongFinished -= HandleSongFinished;
        CameraOrbit.Instance.SetLookEnabled(true);
        isRunning = false;
        reelWheelUI.SetActive(false);
        MusicController.Instance.StopMusic();

    }

   
    private void HandleSongFinished()
    {
        StopReelWheel();

        if (!playBonus)
        {

            OnReelComplete?.Invoke(false);
        }
        else
        {
            OnReelComplete?.Invoke(true);
        }
        playBonus = false;
        
    }

    public bool UpdateFishDistance(bool hit, out bool wasAbsorbed)
    {
        wasAbsorbed = false;
        int result;

        if (hit)
        {
            result = fishMeter.Advance(hit, progressAmount);
        }
        else
        {
            float effectivePenalty = penaltyAmount;
            foreach (var effect in playerEquipment.GetActiveEffects())
            {
                effectivePenalty = effect.OnNoteMissed(fishingController, effectivePenalty);
            }

            wasAbsorbed = effectivePenalty <= 0f; // fully shielded - no real penalty applied
            result = fishMeter.Advance(hit, effectivePenalty);
        }

        if (result == 0)
        {
            return false;
        }

        if (result == -1)
        {
            playBonus = false;
            StopReelWheel();
            Debug.Log("Fish got away!");
            OnReelComplete?.Invoke(false);
        }
        else if (result == 1)
        {
            Debug.Log("Fish caught!");
            playBonus = true;
            return true;
        }
        playBonus = false;
        return false;
    }

    public void SetBPM(float bpm)
    {
        this.bpm = bpm;
    }
}