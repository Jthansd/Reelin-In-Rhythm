using NUnit.Framework;
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
    private float fishDistance = 1f; // 1 = far, 0 = caught

    private bool PlayReelWheel = false;

    public GameObject noteDestroyer;


    [Header("Music Sync")]
    public float bpm = 130f;
    public float beatsPerMeasure = 4f;
    public float rotationOffset = 0f;

    public float accumulatedAngle = 0f;

    [Header("Hold Note Visuals")]
    [SerializeField] private Color holdNoteColor = new Color(0.2f, 0.5f, 1f, 1f); // blue
    [SerializeField] private float holdBarThickness = 8f;

    [SerializeField] private NoteHitManager noteHitManager;

    [SerializeField] private FishMeter fishMeter;

    public bool isCaught = false;

    void Start()
    {
        reelWheelUI.SetActive(false);
        Debug.Log("Reel Wheel Test Started");
    }

    void Update()
    {
        if (PlayReelWheel)
        {
            RunOrbit();
            noteHitManager.SpawnNotesOnTime();
            noteHitManager.CheckInput();
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

            // Give the head references so it can take them down together on OnDestroy.
            // (SpawnHoldTail is changed below to also return the tail GameObject via an out param.)

        }

        noteHitManager.RegisterNote(note);
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

    public void IsCaught()
    {
        isCaught = true;
    }

    public void StartReelWheel()
    {
        isCaught = false;
        fishMeter.ResetProgress();
        reelWheelUI.SetActive(true);
        angle = 0f;
        fishDistance = 1f;
        noteHitManager.ResetNotes();
        noteHitManager.GetSpawnTimes();
        PlayReelWheel = true;
        MusicController.Instance.PlayMusic();
    }

    public void StopReelWheel()
    {
        PlayReelWheel = false;
        reelWheelUI.SetActive(false);
    }

    public bool IsPlaying()
    {
        return PlayReelWheel;
    }

    private void OnEnable()
    {
        MusicController.Instance.OnSongFinished += HandleSongFinished;
    }

    private void OnDisable()
    {
        MusicController.Instance.OnSongFinished -= HandleSongFinished;
    }

    private void HandleSongFinished()
    {
        StopReelWheel();
    }

    public void UpdateFishDistance(bool hit)
    {
        if (hit)
        {
            if (fishMeter.Advance())
            {
                Debug.Log("Fish caught!");
                StopReelWheel();
                IsCaught();
                return;
            }
        }
        else
        {
            fishMeter.Decay();
        }
    }
}