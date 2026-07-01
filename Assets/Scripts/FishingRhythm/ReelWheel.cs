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
            //CheckInput();
            noteHitManager.SpawnNotesOnTime();
            noteHitManager.CheckInput();
            //SpawnNotesOnTime();
        }
    }

    private void RunOrbit()
    {
        //start wheel spinning

        // Calculate the rotation speed based on BPM and beats per measure
        //60 seconds per minute / BPM = seconds per beat
        float secondsPerBeat = 60f / bpm;

        // seconds per beat * beats per measure = seconds per measure
        float secondsPerMeasure = secondsPerBeat * beatsPerMeasure;

        // 360 degrees per measure / seconds per measure = degrees per second
        float degreesPerSecond = 360f / secondsPerMeasure;

        // Update the angle based on the rotation speed and delta time
        angle -= degreesPerSecond * Time.deltaTime;
        angle = (angle + 360f) % 360f;

        //
        float rotationOffset = 90f;

        wheel.localRotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    public void SpawnSingleNote()
    {

        //responsible for spawning a single note on the reel wheel at the spawn point


        GameObject note = Instantiate(notePrefab, wheel);
        RectTransform noteRect = note.GetComponent<RectTransform>();

        // position at spawnpoint (your fixed spawner)
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wheel,
            Spawnpoint.position,
            null,
            out localPos
        );
        noteRect.anchoredPosition = localPos;

        // configure despawn distance for this note
        var noteComp = note.GetComponent<ReelWheelNote>();

        float secondsPerBeat = 60f / bpm;
        float secondsPerMeasure = secondsPerBeat * beatsPerMeasure;
        float degreesPerSecond = 360f / secondsPerMeasure;

        noteComp.travelTime = secondsPerBeat * 4;

        noteHitManager.RegisterNote(note);
    }


    //private void CheckInput()
    //{
    //    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //    {
    //        float delta = Mathf.DeltaAngle(angle, 0f);

    //        if (Mathf.Abs(delta) < hitWindow)
    //        {
    //            Debug.Log("HIT!");
    //            fishDistance -= 0.1f;
    //        }
    //        else
    //        {
    //            Debug.Log("MISS!");
    //            fishDistance += 0.05f;
    //        }

    //        fishDistance = Mathf.Clamp01(fishDistance);

    //        if (fishDistance <= 0f)
    //        {
    //            Debug.Log("Fish caught! (Test Mode)");
    //            StopReelWheel();
    //        }
    //    }
    //}

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