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

    private List<List<float>> spawnTimes = new List<List<float>>();

    private int nextNoteIndex = 0;
    private float spawnLeadTime;

    public float accumulatedAngle = 0f;

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
            CheckInput();
            SpawnNotesOnTime();
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

    private void SpawnSingleNote()
    {
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

        noteComp.degreesToTravel = 360f;
    }


    private void SpawnNotesOnTime()
    {
        List<float> easyTimes = spawnTimes[0];

        if (nextNoteIndex >= easyTimes.Count)
            return;

        float currentTime = MusicController.Instance.GetCurrentMusicTime();
        float hitTime = easyTimes[nextNoteIndex];

        // Spawn 2 beats early
        if (currentTime >= hitTime - spawnLeadTime)
        {
            SpawnSingleNote();
            nextNoteIndex++;
        }
    }


    private void CheckInput()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            float delta = Mathf.DeltaAngle(angle, 0f);

            if (Mathf.Abs(delta) < hitWindow)
            {
                Debug.Log("HIT!");
                fishDistance -= 0.1f;
            }
            else
            {
                Debug.Log("MISS!");
                fishDistance += 0.05f;
            }

            fishDistance = Mathf.Clamp01(fishDistance);

            if (fishDistance <= 0f)
            {
                Debug.Log("Fish caught! (Test Mode)");
                StopReelWheel();
            }
        }
    }

    public bool IsCaught()
    {
        return fishDistance <= 0f;
    }

    public void StartReelWheel()
    {
        spawnLeadTime = 30f / bpm;
        GetSpawnTimes();
        reelWheelUI.SetActive(true);
        angle = 0f;
        fishDistance = 1f;
        PlayReelWheel = true;
        MusicController.Instance.PlayMusic();
    }

    public void StopReelWheel()
    {
        PlayReelWheel = false;
        reelWheelUI.SetActive(false);
    }

    private void GetSpawnTimes()
    {
        spawnTimes.Clear();
        spawnTimes.Add(MusicController.Instance.GetNoteTimingsForDifficulty("easy"));
        spawnTimes.Add(MusicController.Instance.GetNoteTimingsForDifficulty("medium"));
        spawnTimes.Add(MusicController.Instance.GetNoteTimingsForDifficulty("hard"));
    }
}